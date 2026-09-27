using System.Globalization;
using Microsoft.EntityFrameworkCore;
using NorthboundSessions.Data;
using NorthboundSessions.Web.Data;

namespace NorthboundSessions.Web.Services;

/// <summary>An achievement plus whether the student has earned it, and how far along they are.</summary>
/// <param name="Current">Progress toward <see cref="AchievementDefinition.Goal"/>; 0 when not measurable.</param>
public sealed record AchievementState(AchievementDefinition Definition, bool Unlocked, int Current = 0)
{
    /// <summary>Completion as a percentage, for the progress bar on locked achievements.</summary>
    public int Percent => Definition.Goal <= 0
        ? 0
        : Math.Clamp((int)Math.Round(Current * 100.0 / Definition.Goal), 0, 100);
}

/// <summary>Aggregate counts the unlock rules are evaluated against.</summary>
public sealed record AchievementStats
{
    public int AttemptCount { get; init; }
    public int DistinctLessonsAttempted { get; init; }
    public int ReleasedLessonCount { get; init; }
    public int PerfectScoreCount { get; init; }
    public int HighScoreCount { get; init; }          // >= 90%
    public int SolidScoreCount { get; init; }         // >= 80%
    public int AttendanceCount { get; init; }
    public int DistinctSessionsHeld { get; init; }
    public int CurrentStreak { get; init; }
    public int DistinctActiveDays { get; init; }
    public int BestWeekDayCount { get; init; }
    public int DistinctSymbolsAttempted { get; init; }

    /// <summary>Most distinct lessons studied that share one market symbol.</summary>
    public int MaxLessonsPerSymbol { get; init; }

    /// <summary>Quizzes where a later attempt scored higher than an earlier one.</summary>
    public int RetakeImprovements { get; init; }
    public bool DidEarlyBird { get; init; }
    public bool DidNightOwl { get; init; }
    public bool DidWeekend { get; init; }
    public bool DidComeback { get; init; }
}

/// <summary>
/// Evaluates <see cref="AchievementCatalog"/> against a student's real activity.
/// Extracted from Dashboard.razor, which previously hardcoded five achievements
/// and inlined the icons as MarkupString.
/// </summary>
public class AchievementService(IDbContextFactory<ApplicationDbContext> dbFactory)
{
    /// <summary>Loads the student's activity and returns every achievement with its unlock state.</summary>
    public async Task<IReadOnlyList<AchievementState>> GetAchievementsAsync(
        string studentId,
        CancellationToken ct = default)
    {
        var stats = await BuildStatsAsync(studentId, ct);
        var results = new List<AchievementState>(AchievementCatalog.All.Count);

        // (isUnlocked, current) for each achievement, keyed by id.
        (bool, int) Eval(string id) => id switch
        {
            "first-quiz"     => (stats.AttemptCount >= 1, stats.AttemptCount),
            "first-session"  => (stats.AttendanceCount >= 1, stats.AttendanceCount),
            "five-lessons"   => (stats.DistinctLessonsAttempted >= 5, stats.DistinctLessonsAttempted),
            "ten-lessons"    => (stats.DistinctLessonsAttempted >= 10, stats.DistinctLessonsAttempted),
            "twentyfive"     => (stats.DistinctLessonsAttempted >= 25, stats.DistinctLessonsAttempted),
            "completionist"  => (stats.ReleasedLessonCount > 0
                                && stats.DistinctLessonsAttempted >= stats.ReleasedLessonCount,
                                stats.DistinctLessonsAttempted),

            "streak-3"       => (stats.CurrentStreak >= 3, stats.CurrentStreak),
            "streak-7"       => (stats.CurrentStreak >= 7, stats.CurrentStreak),
            "streak-30"      => (stats.CurrentStreak >= 30, stats.CurrentStreak),
            "streak-100"     => (stats.CurrentStreak >= 100, stats.CurrentStreak),
            "comeback"       => (stats.DidComeback, 0),
            "five-day-week"  => (stats.BestWeekDayCount >= 5, stats.BestWeekDayCount),

            "perfect"        => (stats.PerfectScoreCount >= 1, stats.PerfectScoreCount),
            "sharp"          => (stats.HighScoreCount >= 1, stats.HighScoreCount),
            "consistent-80"  => (stats.SolidScoreCount >= 5, stats.SolidScoreCount),
            "ace-x3"         => (stats.PerfectScoreCount >= 3, stats.PerfectScoreCount),

            "quizzes-10"     => (stats.AttemptCount >= 10, stats.AttemptCount),
            "quizzes-50"     => (stats.AttemptCount >= 50, stats.AttemptCount),
            "quizzes-100"    => (stats.AttemptCount >= 100, stats.AttemptCount),

            "sessions-5"     => (stats.AttendanceCount >= 5, stats.AttendanceCount),
            "sessions-20"    => (stats.AttendanceCount >= 20, stats.AttendanceCount),
            "perfect-attend" => (stats.DistinctSessionsHeld > 0
                                && stats.AttendanceCount >= stats.DistinctSessionsHeld,
                                stats.AttendanceCount),

            "early-bird"     => (stats.DidEarlyBird, 0),
            "night-owl"      => (stats.DidNightOwl, 0),
            "weekend-warrior" => (stats.DidWeekend, 0),
            "marathon"       => (stats.DistinctActiveDays >= 10, stats.DistinctActiveDays),

            "diversified"    => (stats.DistinctSymbolsAttempted >= 5, stats.DistinctSymbolsAttempted),
            "symbol-scholar" => (stats.MaxLessonsPerSymbol >= 3, stats.MaxLessonsPerSymbol),
            "retake-improved" => (stats.RetakeImprovements > 0, stats.RetakeImprovements),

            // Resolved after the loop, once every other result is known.
            "market-wizard"  => (false, 0),
            _                => (false, 0),
        };

        foreach (var def in AchievementCatalog.All)
        {
            var (unlocked, current) = Eval(def.Id);
            results.Add(new AchievementState(def, unlocked, current));
        }

        // Market Wizard unlocks only when every other achievement is earned.
        var allOthers = results.Where(r => r.Definition.Id != "market-wizard").All(r => r.Unlocked);
        var othersUnlocked = results.Count(r => r.Definition.Id != "market-wizard" && r.Unlocked);

        return results
            .Select(r => r.Definition.Id == "market-wizard"
                ? r with { Unlocked = allOthers, Current = othersUnlocked }
                : r)
            .ToList();
    }

    private async Task<AchievementStats> BuildStatsAsync(string studentId, CancellationToken ct)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);

        var releasedLessonCount = await db.Lessons
            .CountAsync(l => l.ReleaseDate <= DateOnly.FromDateTime(DateTime.UtcNow), ct);

        if (string.IsNullOrEmpty(studentId))
        {
            return new AchievementStats { ReleasedLessonCount = releasedLessonCount };
        }

        var attempts = await db.QuizAttempts
            .Where(a => a.StudentId == studentId)
            .ToListAsync(ct);

        var attendances = await db.Attendances
            .Where(a => a.StudentId == studentId)
            .ToListAsync(ct);

        // "Attend every session held so far" must only count sessions that have
        // already started. Counting all LiveSessions included future ones, so
        // this achievement could not be earned until the schedule ran out.
        var now = DateTime.UtcNow;
        var sessionsHeld = await db.LiveSessions.CountAsync(s => s.ScheduledAt <= now, ct);

        if (attempts.Count == 0 && attendances.Count == 0)
        {
            return new AchievementStats { ReleasedLessonCount = releasedLessonCount };
        }

        // Correct answers per quiz, so scores can be turned into percentages.
        var quizIds = attempts.Select(a => a.QuizId).Distinct().ToList();
        var questionCounts = await db.QuizQuestions
            .Where(q => quizIds.Contains(q.QuizId))
            .GroupBy(q => q.QuizId)
            .Select(g => new { QuizId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.QuizId, x => x.Count, ct);

        var percents = new List<double>();
        foreach (var a in attempts)
        {
            if (questionCounts.TryGetValue(a.QuizId, out var total) && total > 0)
            {
                percents.Add(a.Score * 100.0 / total);
            }
        }

        // Lessons each attempt belongs to, so we can count distinct lessons/symbols.
        var lessonIdsByQuiz = await db.Quizzes
            .Where(q => quizIds.Contains(q.Id))
            .Select(q => new { q.Id, LessonId = q.LessonId })
            .ToDictionaryAsync(x => x.Id, x => x.LessonId, ct);

        var attemptedLessonIds = attempts
            .Select(a => lessonIdsByQuiz.TryGetValue(a.QuizId, out var lid) ? lid : (int?)null)
            .Where(l => l.HasValue)
            .Select(l => l!.Value)
            .Distinct()
            .ToList();

        var symbols = await db.Quizzes
            .Where(q => lessonIdsByQuiz.Values.Contains(q.LessonId))
            .Select(q => q.Lesson!.MarketSymbol)
            .Where(s => s != null)
            .Distinct()
            .CountAsync(ct);

        // Depth, complementing `diversified` (breadth): how many distinct
        // lessons the student has studied on a single symbol. Grouped in
        // memory because it spans two already-loaded collections.
        var symbolByLesson = await db.Lessons
            .Where(l => lessonIdsByQuiz.Values.Contains(l.Id))
            .Select(l => new { l.Id, Symbol = l.MarketSymbol })
            .ToDictionaryAsync(x => x.Id, x => x.Symbol, ct);

        var maxLessonsPerSymbol = attemptedLessonIds
            .Where(lessonId => symbolByLesson.TryGetValue(lessonId, out var s) && !string.IsNullOrEmpty(s))
            .GroupBy(lessonId => symbolByLesson[lessonId])
            .Select(g => g.Distinct().Count())
            .DefaultIfEmpty(0)
            .Max();

        // A retake improvement = a later attempt on the same quiz outscored an
        // earlier one. Compared in points, not percent, but both attempts on a
        // given quiz have the same question count, so the ordering is identical.
        var retakeImprovements = attempts
            .GroupBy(a => a.QuizId)
            .Where(g => g.Count() > 1)
            .Select(g =>
            {
                var ordered = g.OrderBy(a => a.SubmittedAt).Select(a => a.Score).ToList();
                return ordered
                    .Zip(ordered.Skip(1), (prev, next) => next > prev)
                    .Any(improved => improved);
            })
            .Count(beatFirstTry => beatFirstTry);

        // Activity days drive streaks, "active days" and the weekly count.
        var activityDays = attempts.Select(a => a.SubmittedAt.UtcDateTime.Date)
            .Concat(attendances.Select(a => a.CheckedInAt.UtcDateTime.Date))
            .Distinct()
            .OrderByDescending(d => d)
            .ToList();

        var (currentStreak, _) = ComputeStreaks(activityDays);

        // A comeback = a run of >= 3 days that is not at the head of the history,
        // i.e. the student broke a streak and then rebuilt one.
        var didComeback = HasComebackRun(activityDays);

        // Best single week: group activity days by the Monday that starts their week.
        var bestWeekDays = 0;
        foreach (var group in activityDays.GroupBy(d =>
                     ISOWeek.ToDateTime(ISOWeek.GetYear(d), ISOWeek.GetWeekOfYear(d), DayOfWeek.Monday)))
        {
            bestWeekDays = Math.Max(bestWeekDays, group.Count());
        }

        return new AchievementStats
        {
            AttemptCount = attempts.Count,
            DistinctLessonsAttempted = attemptedLessonIds.Count,
            ReleasedLessonCount = releasedLessonCount,
            PerfectScoreCount = percents.Count(p => p >= 99.5),
            HighScoreCount = percents.Count(p => p >= 90),
            SolidScoreCount = percents.Count(p => p >= 80),
            AttendanceCount = attendances.Count,
            DistinctSessionsHeld = sessionsHeld,
            CurrentStreak = currentStreak,
            DistinctActiveDays = activityDays.Count,
            BestWeekDayCount = bestWeekDays,
            DistinctSymbolsAttempted = symbols,
            MaxLessonsPerSymbol = maxLessonsPerSymbol,
            RetakeImprovements = retakeImprovements,
            DidEarlyBird = attempts.Any(a => a.SubmittedAt.UtcDateTime.Hour < 8),
            DidNightOwl = attempts.Any(a => a.SubmittedAt.UtcDateTime.Hour >= 18),
            DidWeekend = attempts.Any(a => IsWeekend(a.SubmittedAt.UtcDateTime)),
            DidComeback = didComeback,
        };
    }

    private static bool IsWeekend(DateTime d) => d.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;

    /// <summary>
    /// Current streak = consecutive days ending today or yesterday.
    /// Longest streak = the longest consecutive run in the whole history.
    /// </summary>
    internal static (int current, int longest) ComputeStreaks(List<DateTime> datesDescending)
    {
        if (datesDescending.Count == 0) return (0, 0);

        var today = DateTime.UtcNow.Date;
        var set = datesDescending.ToHashSet();

        var cursor = set.Contains(today) ? today : today.AddDays(-1);
        var current = 0;
        while (set.Contains(cursor))
        {
            current++;
            cursor = cursor.AddDays(-1);
        }

        var longest = 1;
        var run = 1;
        for (int i = 1; i < datesDescending.Count; i++)
        {
            if (datesDescending[i - 1].AddDays(-1) == datesDescending[i])
            {
                run++;
                longest = Math.Max(longest, run);
            }
            else
            {
                run = 1;
            }
        }

        return (current, longest);
    }

    /// <summary>
    /// True when the student built a streak of 3+ days, let it lapse, and then
    /// built another one of 3+ days. Implemented as "two separate runs of &gt;= 3",
    /// which is unambiguous: a single unbroken run cannot satisfy it.
    /// </summary>
    private static bool HasComebackRun(List<DateTime> datesDescending)
    {
        if (datesDescending.Count < 7) return false;

        var qualifyingRuns = 0;
        var run = 1;

        for (int i = 1; i < datesDescending.Count; i++)
        {
            if (datesDescending[i - 1].AddDays(-1) == datesDescending[i])
            {
                run++;
            }
            else
            {
                if (run >= 3) qualifyingRuns++;
                if (qualifyingRuns >= 2) return true;
                run = 1;
            }
        }

        // The final (oldest) run is not followed by a gap, so close it out here.
        return run >= 3 && qualifyingRuns >= 1;
    }
}
