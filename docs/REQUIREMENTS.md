# Requirements

## Functional requirements

- Students can register, log in, and view their assigned lessons
- A new lesson (with generated slides and a handout) is released
  automatically each weekday, without manual action
- Students can complete a short quiz per lesson and receive an instant score
- Two live sessions per week are scheduled, with a join link visible to
  enrolled students and an in-app attendance check-in
- Instructors can view student progress: quiz scores, attendance, lessons
  completed
- Instructors can edit landing page content (bios, testimonials, FAQ,
  curriculum) without a code change or deployment
- The system supports more than one instructor profile without a schema
  change

## Non-functional requirements

- **Cost**: the system must run at $0/month at a class size under 20
  students, using only free-tier cloud services
- **Reliability**: automated lesson generation must complete before the
  relevant school day begins; a failure must be visible (logged/alerted),
  not silent
- **Class size**: designed for a cohort under 20 students; no requirement to
  support horizontal scaling beyond that
- **Availability**: brief cold-start delays are acceptable; 24/7
  always-warm availability is not a requirement
- **Security**: student data and quiz results are only visible to that
  student and instructors; connection strings and secrets are never
  committed to source control
- **Maintainability**: a solo, part-time developer must be able to
  understand and modify any part of the system without needing to hold the
  whole codebase in memory at once

## Explicitly out of scope (for now)

- Real brokerage/trading integration — the practice simulator, if built,
  uses fake balances and delayed or free-tier market data only
- Payment processing
- Support for cohorts larger than ~20 students
