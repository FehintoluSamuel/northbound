import { existsSync, readFileSync } from "node:fs";
import { dirname, join } from "node:path";
import { createRealtime, rawSql } from "@neon/realtime/server";

const APP_URL = process.env.NORTHBOUND_APP_URL ?? "http://localhost:5198";
const CHECK_TIMEOUT_MS = 60_000;

function unquote(value) {
  if (value.length >= 2 && ((value[0] === '"' && value.at(-1) === '"') || (value[0] === "'" && value.at(-1) === "'"))) {
    return value.slice(1, -1);
  }
  return value;
}

function loadEnv(path) {
  for (const line of readFileSync(path, "utf8").split(/\r?\n/)) {
    const trimmed = line.trim();
    if (!trimmed || trimmed.startsWith("#")) continue;
    const separator = trimmed.indexOf("=");
    if (separator <= 0) continue;
    const key = trimmed.slice(0, separator).trim();
    if (process.env[key]) continue;
    process.env[key] = unquote(trimmed.slice(separator + 1).trim());
  }
}

function findEnv() {
  let directory = process.cwd();
  while (true) {
    const candidate = join(directory, ".env.local");
    if (existsSync(candidate)) return candidate;
    const parent = dirname(directory);
    if (parent === directory) return null;
    directory = parent;
  }
}

function required(name) {
  const value = process.env[name];
  if (!value) throw new Error(`${name} is missing from environment or .env.local`);
  return value;
}

function databaseName(connectionString) {
  return new URL(connectionString).pathname.replace(/^\//, "") || "neondb";
}

const envPath = findEnv();
if (envPath) loadEnv(envPath);

const realtime = createRealtime({
  url: required("NEON_REALTIME_URL"),
  secret: required("NEON_REALTIME_SECRET"),
  db: databaseName(required("DATABASE_URL"))
});

const query = rawSql(
  `SELECT "Id" AS id, "Message" AS message, "UpdatedAtUtc" AS updated_at_utc
   FROM "RealtimeProbes"
   ORDER BY "Id" DESC
   LIMIT 5`
);

let resolveInitial;
let rejectInitial;
let resolveChange;
let rejectChange;
let expectedMessage;
let initialRows;

const initial = new Promise((resolve, reject) => {
  resolveInitial = resolve;
  rejectInitial = reject;
});
const changed = new Promise((resolve, reject) => {
  resolveChange = resolve;
  rejectChange = reject;
});
initial.catch(() => {});
changed.catch(() => {});

function fail(error) {
  const failure = error instanceof Error ? error : new Error(String(error));
  rejectInitial(failure);
  rejectChange(failure);
}

function inspectSnapshot(snapshot) {
  if (snapshot.status === "error") {
    fail(snapshot.error);
    return;
  }

  if (snapshot.status === "live" && snapshot.data !== undefined && !initialRows) {
    initialRows = snapshot.data;
    resolveInitial(snapshot);
  }

  if (expectedMessage && snapshot.data?.some((row) => row.message === expectedMessage)) {
    resolveChange(snapshot);
  }
}

const timeout = setTimeout(
  () => fail(new Error(`Realtime check timed out after ${CHECK_TIMEOUT_MS / 1000} seconds`)),
  CHECK_TIMEOUT_MS
);

let subscription;
try {
  subscription = await realtime.subscribe(query);
  subscription.onStateChange((state) => {
    console.log(`Realtime status: ${state.status}`);
    if (state.status === "error") fail(state.error);
  });
  subscription.onChange(inspectSnapshot);

  await initial;
  console.log(`Realtime initial snapshot: ${initialRows.length} row(s)`);

  const writeResponse = await fetch(`${APP_URL}/api/realtime/probe`, { method: "POST" });
  if (!writeResponse.ok) {
    throw new Error(`Probe write endpoint returned ${writeResponse.status}`);
  }

  const payload = await writeResponse.json();
  expectedMessage = payload.message;

  const readResponse = await fetch(`${APP_URL}/api/realtime/probe`);
  if (!readResponse.ok) {
    throw new Error(`Probe read endpoint returned ${readResponse.status}`);
  }

  const storedRows = await readResponse.json();
  console.log(`Postgres write endpoint: id=${payload.id}`);
  console.log(`Stored probe rows: ${storedRows.length}`);

  const poll = setInterval(() => inspectSnapshot(subscription.getSnapshot()), 500);
  try {
    await changed;
  } finally {
    clearInterval(poll);
  }

  console.log(`Realtime change observed: ${payload.message}`);
} finally {
  clearTimeout(timeout);
  subscription?.unsubscribe();
  await realtime.close();
}
