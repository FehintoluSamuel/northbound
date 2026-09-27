namespace NorthboundSessions.Web.Services;

/// <summary>Visual tier of an achievement. Drives the badge colour via CSS tokens.</summary>
public enum AchievementTier
{
    None = 0,
    Bronze = 1,
    Silver = 2,
    Gold = 3,
}

/// <summary>One defined achievement. The unlock rule is evaluated by <see cref="AchievementService"/>.</summary>
/// <param name="Id">Stable key — safe to persist; do not rename once awarded.</param>
public sealed record AchievementDefinition(
    string Id,
    string Name,
    string Description,
    string Icon,
    AchievementTier Tier,
    int Goal);

/// <summary>
/// The full achievement catalog. Adding an entry here is enough for it to appear
/// in the UI — no migration needed, since nothing is persisted yet.
/// </summary>
public static class AchievementCatalog
{
    public static readonly IReadOnlyList<AchievementDefinition> All = new[]
    {
        // --- Getting started -------------------------------------------------
        new AchievementDefinition("first-quiz",     "First Steps",    "Submit your first quiz",                    "sprout",         AchievementTier.Bronze, 1),
        new AchievementDefinition("first-session",  "Front Row",      "Check in to your first live session",       "calendar-check", AchievementTier.Bronze, 1),
        new AchievementDefinition("five-lessons",   "Getting Started","Complete quizzes on 5 different lessons",   "book",           AchievementTier.Bronze, 5),
        new AchievementDefinition("ten-lessons",    "Bookworm",       "Complete quizzes on 10 different lessons",  "library",        AchievementTier.Bronze, 10),
        new AchievementDefinition("twentyfive",     "Scholar",        "Complete quizzes on 25 different lessons",  "graduation",     AchievementTier.Silver, 25),
        new AchievementDefinition("completionist",  "Completionist",  "Complete a quiz on every released lesson",  "graduation",     AchievementTier.Gold,   0),

        // --- Streaks ---------------------------------------------------------
        new AchievementDefinition("streak-3",       "Warm Up",        "Stay active 3 days in a row",               "flame",          AchievementTier.Bronze, 3),
        new AchievementDefinition("streak-7",       "Week Strong",    "Stay active 7 days in a row",               "flame",          AchievementTier.Silver, 7),
        new AchievementDefinition("streak-30",      "Month Strong",   "Stay active 30 days in a row",              "flame",          AchievementTier.Gold,   30),
        new AchievementDefinition("streak-100",     "Century Streak", "Stay active 100 days in a row",             "crown",          AchievementTier.Gold,   100),
        new AchievementDefinition("comeback",       "Comeback Kid",   "Rebuild a 3-day streak after breaking one", "repeat",         AchievementTier.Silver, 3),
        new AchievementDefinition("five-day-week",  "Full Week",      "Be active on 5 separate days in one week",  "calendar-check", AchievementTier.Silver, 5),

        // --- Accuracy --------------------------------------------------------
        new AchievementDefinition("perfect",        "Bullseye",       "Score 100% on any quiz",                    "target",         AchievementTier.Bronze, 100),
        new AchievementDefinition("sharp",          "Sharp Shooter",  "Score 90% or better on a quiz",             "crosshair",      AchievementTier.Silver, 90),
        new AchievementDefinition("consistent-80",  "Steady Hand",    "Score 80%+ on 5 different quizzes",         "scale",          AchievementTier.Silver, 5),
        new AchievementDefinition("ace-x3",         "Triple Threat",  "Score 100% on 3 different quizzes",         "gem",            AchievementTier.Gold,   3),

        // --- Volume ----------------------------------------------------------
        new AchievementDefinition("quizzes-10",     "Quiz Whiz",      "Submit 10 quiz attempts",                   "medal",          AchievementTier.Bronze, 10),
        new AchievementDefinition("quizzes-50",     "Quiz Champion",  "Submit 50 quiz attempts",                   "trophy",         AchievementTier.Silver, 50),
        new AchievementDefinition("quizzes-100",    "Centurion",      "Submit 100 quiz attempts",                  "trophy",         AchievementTier.Gold,   100),

        // --- Attendance ------------------------------------------------------
        new AchievementDefinition("sessions-5",     "Regular",        "Check in to 5 live sessions",               "calendar-check", AchievementTier.Bronze, 5),
        new AchievementDefinition("sessions-20",    "Devoted",        "Check in to 20 live sessions",              "clock",          AchievementTier.Silver, 20),
        new AchievementDefinition("perfect-attend", "Full House",     "Attend every session held so far",          "shield",         AchievementTier.Gold,   0),

        // --- Habits ----------------------------------------------------------
        new AchievementDefinition("early-bird",     "Early Bird",     "Submit a quiz before 8 AM",                 "sunrise",        AchievementTier.Bronze, 1),
        new AchievementDefinition("night-owl",      "Night Owl",      "Submit a quiz after 6 PM",                  "moon",           AchievementTier.Bronze, 1),
        new AchievementDefinition("weekend-warrior","Weekend Warrior","Study on a Saturday or Sunday",            "bolt",           AchievementTier.Bronze, 1),
        new AchievementDefinition("marathon",       "Deep Dive",      "Be active on 10 different days",            "mountain",       AchievementTier.Silver, 10),

        // --- Markets ---------------------------------------------------------
        new AchievementDefinition("diversified",    "Diversified",    "Complete quizzes on lessons covering 5 different symbols", "globe",    AchievementTier.Silver, 5),
        new AchievementDefinition("symbol-scholar", "Symbol Scholar", "Study 3 different lessons on the same market symbol",    "eye",     AchievementTier.Silver, 3),
        new AchievementDefinition("retake-improved", "Second Wind",   "Retake a quiz and beat your first attempt",  "trending-up",    AchievementTier.Bronze, 1),

        // --- Ultimate --------------------------------------------------------
        new AchievementDefinition("market-wizard",  "Market Wizard",  "Unlock every other achievement",            "sparkles",       AchievementTier.Gold,   0),
    };

    public static AchievementDefinition? ById(string id) =>
        All.FirstOrDefault(a => a.Id == id);
}
