namespace NorthboundSessions.Web.Services;

public static class LessonBankContent
{
    public static IReadOnlyList<LessonSeed> Items { get; } = new[]
    {
        new LessonSeed(
            Title: "Lesson 2 — Reading a Chart: What Price Is Actually Telling You",
            MarketSymbol: "AMEX:SPY",
            Slides: new[]
            {
                """
                A chart is not a crystal ball. It is a picture of a conversation between buyers and sellers.

                Think of it like a weather map. The map does not create the rain, and it cannot promise where the next cloud will go. It helps you notice where the wind is coming from, where the storm is gathering, and whether the road you are about to take needs extra care.

                We are not trying to read minds here. We are learning to describe what price has already done, so our decisions are based on something visible instead of a hunch.
                """,
                """
                One candle is one small argument about value.

                A candle shows four things: the price where the period opened, the highest price reached, the lowest price reached, and the price where it closed. The thick middle part is called the body. The thin lines above and below are called wicks.

                Imagine a stock opens at $100, falls to $97, climbs to $106, and closes at $104. That candle tells us sellers pushed early, buyers took over, and buyers ended the round in charge. The green or red colour is just a quick way to see who won that round. It is not a moral judgement and it is not a prediction.
                """,
                """
                A trend is a direction, not a promise.

                If a stock keeps making higher highs and higher lows, we call that an uptrend. If it keeps making lower highs and lower lows, we call that a downtrend. If it keeps bouncing between roughly the same ceiling and floor, we call that a range.

                A trend is like the current in a river. It can help you understand which way the water is generally moving, but it does not mean you can step in without looking. Currents change. Rivers bend. A trend is useful because it gives you context, not because it guarantees the next step.
                """,
                """
                Support and resistance are remembered prices.

                Support is an area where buyers have shown up before and stopped a fall. Resistance is an area where sellers have shown up before and stopped a rise. Picture a ball bouncing in a room. The floor is support. The ceiling is resistance. Neither one is magic. They are just places where the room has behaved the same way more than once.

                If a stock turned around near $100 twice, people notice. Some of them place orders there again. That does not make $100 sacred. It simply makes it a place worth watching.
                """,
                """
                Volume tells you how crowded the room was.

                Volume is the number of shares traded during a period. A price move on high volume means many buyers and sellers agreed to do business at those prices. A price move on thin volume means very few people were involved.

                Think of a football stadium. A goal celebrated by a packed crowd feels different from a goal in an empty practice ground. Same event, very different weight. A breakout above resistance on strong volume is more interesting than the same breakout on a quiet Tuesday afternoon when nobody was watching.
                """,
                """
                A moving average is smoothed history.

                A 50-day moving average adds up the last 50 closing prices and divides by 50. Then it does that again every day. The line moves more slowly than price because it is carrying the recent past with it.

                You can think of it as the average mood of the market over the last couple of months. If price is above that line, the recent mood has been fairly positive. If price is below it, the mood has cooled. The line does not push price around. It simply helps us see the slope underneath all the daily noise.
                """,
                """
                Patterns are descriptions, not prophecies.

                You will hear names like flags, triangles, head and shoulders, and double bottoms. These are shapes people noticed often enough to give them names. They can be useful shorthand, but they do not work because the shape itself has power.

                A pattern works only when the behaviour behind it keeps showing up. Buyers keep defending a level. Sellers keep losing control. Volume confirms that more people are joining. Without that behaviour, a pattern is just a drawing on a chart, like seeing a face in a cloud.
                """,
                """
                Time frames change the story you see.

                A five-minute chart can look like a storm. The same stock on a daily chart can look like a calm walk. Neither chart is lying. They are simply different zoom levels, just like a city map and a world map.

                Before you judge a chart, decide what kind of decision you are making. If you are learning to hold a position for weeks, a five-minute wobble should not shake your plan. If you are trading a short live-session example, a monthly chart may be too far away to help. Pick the map that matches the route.
                """,
                """
                Here is a simple routine you can reuse every time.

                1. Name the time frame you are looking at.
                2. Say the trend out loud: up, down, or sideways.
                3. Mark the nearest support and resistance areas.
                4. Check whether volume supports what you think you see.
                5. Ask what news or company event could explain the move.
                6. Decide whether this is a trade, a watch, or nothing at all.

                Notice that "nothing at all" is a real answer. Most charts deserve a watch, not an order.
                """,
                """
                Three traps catch new chart readers again and again.

                The first is the falling knife. A stock has dropped hard and looks cheap, so people catch it on the way down. A falling knife can still be falling. Wait for signs that buyers have actually stepped in.

                The second is chart worship. A line on a screen cannot care about your entry price. If a level breaks, it breaks. Change the plan, not the story.

                The third is seeing what you want to see. Once you want a stock to go up, every candle starts looking bullish. Write down what would prove you wrong before you look for reasons to be right. That one habit will save you more money than any fancy indicator.
                """
            },
            Questions: new[]
            {
                new QuestionSeed(
                    "What does the body of a candlestick show?",
                    new[]
                    {
                        "The highest and lowest prices only",
                        "The opening and closing prices for that period",
                        "The company's profit for the year",
                        "The number of shares available"
                    },
                    CorrectIndex: 1
                ),
                new QuestionSeed(
                    "A stock keeps making higher highs and higher lows. What is that usually called?",
                    new[]
                    {
                        "A downtrend",
                        "A range",
                        "An uptrend",
                        "A reversal"
                    },
                    CorrectIndex: 2
                ),
                new QuestionSeed(
                    "What is support?",
                    new[]
                    {
                        "An area where buyers have previously stopped a fall",
                        "A guarantee that price will never go lower",
                        "The maximum price allowed by the exchange",
                        "A rule that forces you to buy"
                    },
                    CorrectIndex: 0
                ),
                new QuestionSeed(
                    "Why does volume matter when price breaks above resistance?",
                    new[]
                    {
                        "It tells us how many people are participating in the move",
                        "It tells us the company's debt level",
                        "It makes the chart look nicer",
                        "It guarantees the next candle will be green"
                    },
                    CorrectIndex: 0
                ),
                new QuestionSeed(
                    "What is a 50-day moving average?",
                    new[]
                    {
                        "A prediction of tomorrow's price",
                        "The average closing price over the last 50 days",
                        "The price the stock must always return to",
                        "A count of how many people own the stock"
                    },
                    CorrectIndex: 1
                ),
                new QuestionSeed(
                    "What should you do first when reading an unfamiliar chart?",
                    new[]
                    {
                        "Buy immediately before the move starts",
                        "Choose a time frame that matches your decision",
                        "Ignore the chart and follow a friend",
                        "Wait for a pattern name to appear"
                    },
                    CorrectIndex: 1
                )
            }
        ),
        new LessonSeed(
            Title: "Lesson 3 — Risk First: Staying in the Game Long Enough to Learn",
            MarketSymbol: "AMEX:SPY",
            Slides: new[]
            {
                """
                Risk is not a footnote at the end of a lesson. It is the seatbelt.

                Most people learn this the hard way. They find a good idea, feel the excitement of being early, and put on too much. One normal pullback later, the position is painful enough that they sell at the worst time. The idea may have been fine. The size was not.

                Our goal is not to be right on every trade. Our goal is to still be here in five years, with enough capital and calm to keep learning.
                """,
                """
                Before you enter, decide what you can afford to lose.

                Say your trading account has ₦200,000. If you risk 1% on one idea, your maximum loss is ₦2,000. That number is not pessimistic. It is the cost of being wrong on one small idea.

                Now say your entry is ₦100 and your stop is ₦95. You are risking ₦5 per share. To risk ₦2,000, you can buy 400 shares. That is ₦40,000 of stock, not ₦200,000. The position size comes from the risk, not from how excited you feel.
                """,
                """
                A stop-loss is a promise to yourself.

                It is the price where you admit, calmly, that the trade is not working. This is not a punishment and it is not a sign of weakness. It is the fire exit. You hope you never need it, but you would never enter a building without knowing where it is.

                A good stop is tied to the idea, not to a number you picked because it feels tidy. If your reason for buying was a move above resistance, a fall back below that area may mean the idea is broken. That is where the exit belongs.
                """,
                """
                Risk and reward are a trade-off, not a magic ratio.

                If you are risking ₦2,000, asking to make ₦6,000 means you are aiming for three times your risk. That sounds attractive on paper. In real life, you still need a realistic reason to expect that move.

                A target is not a wish. It should come from something on the chart or in the business. Maybe the stock is approaching old resistance. Maybe the company's results could change the story. If you cannot explain the target in one sentence, you do not have a target yet.
                """,
                """
                Position size is the quiet part of trading.

                Two students can both be right about the same stock and have completely different days. One buys a small position and watches calmly. The other buys almost everything and checks the price every thirty seconds. Same view, completely different experience.

                The second student is not brave. They are exposed. When a position is too large, normal market noise feels like an emergency. You make decisions to relieve pressure instead of decisions that follow your plan.
                """,
                """
                Correlation means your different trades may be one big bet.

                If you buy five bank stocks and they all fall when interest rates move, you do not own five different ideas. You own one idea, dressed in five outfits. It is like carrying five umbrellas in the same storm. They look like separate protection, but the rain is coming from one direction.

                Before you add another position, ask what else would be hurt by the same news. If the answer is "most of my portfolio", your risk is larger than it looks.
                """,
                """
                Small losses are tuition. Huge losses are expulsion.

                If you lose 10% of your account, you need about 11% to get back to even. If you lose 50%, you need 100%. That is not a motivational quote. It is arithmetic, and arithmetic does not negotiate.

                This is why we cut small losses quickly. A small loss is a bill for information. A huge loss is a lost year of learning. Keep the bill small enough that you can stay in class.
                """,
                """
                Make a plan for your bad days before they arrive.

                Decide in advance what happens after three losing trades in a session. Maybe you close the laptop. Maybe you walk outside and do not come back until the next day. Write it down while you are calm.

                Revenge trading is what happens when the market bruises your pride and you try to hurt it back. The market does not feel anything. It will happily take another order from you. A stop rule protects you from yourself.
                """,
                """
                Here is the risk checklist before every order.

                1. What is the idea in one sentence?
                2. What price proves the idea wrong?
                3. How much money am I risking if that happens?
                4. How many shares does that risk allow?
                5. What would make me take profit or move the stop?
                6. What else in my portfolio fails if this idea fails?

                If any answer is blank, the trade is not ready. You are not missing an opportunity. You are finishing the work.
                """,
                """
                Risk management does not make you right.

                It makes being wrong survivable. You will still have losing trades. Some will sting even when you followed every rule. That is normal. The market pays for patience, not perfection.

                The win is that a bad trade stays bad instead of turning into a bad month. Protect the account first. The good ideas will still be there tomorrow.
                """
            },
            Questions: new[]
            {
                new QuestionSeed(
                    "If a ₦200,000 account risks 1% on a trade, what is the maximum planned loss?",
                    new[]
                    {
                        "₦200",
                        "₦2,000",
                        "₦20,000",
                        "₦200,000"
                    },
                    CorrectIndex: 1
                ),
                new QuestionSeed(
                    "What is the main purpose of a stop-loss?",
                    new[]
                    {
                        "To guarantee a profit",
                        "To make the broker like you",
                        "To define where the trade idea is wrong",
                        "To remove all market risk"
                    },
                    CorrectIndex: 2
                ),
                new QuestionSeed(
                    "Entry is ₦100, stop is ₦95, and planned risk is ₦2,000. How many shares fit the plan?",
                    new[]
                    {
                        "20 shares",
                        "100 shares",
                        "400 shares",
                        "2,000 shares"
                    },
                    CorrectIndex: 2
                ),
                new QuestionSeed(
                    "Why are five bank stocks often more risky together than they first appear?",
                    new[]
                    {
                        "They have different tickers",
                        "They are all exposed to similar news and sector moves",
                        "Banks never move together",
                        "The chart colours are different"
                    },
                    CorrectIndex: 1
                ),
                new QuestionSeed(
                    "After a 50% loss, what gain is needed just to return to the starting balance?",
                    new[]
                    {
                        "25%",
                        "50%",
                        "75%",
                        "100%"
                    },
                    CorrectIndex: 3
                ),
                new QuestionSeed(
                    "When should you decide your maximum loss on a trade?",
                    new[]
                    {
                        "After the price moves against you",
                        "Before entering the trade",
                        "Only when the market closes",
                        "When a friend tells you to sell"
                    },
                    CorrectIndex: 1
                )
            }
        ),
        new LessonSeed(
            Title: "Lesson 4 — Putting It Together: A Real Stock Review, Step by Step",
            MarketSymbol: "NASDAQ:MSFT",
            Slides: new[]
            {
                """
                This lesson is a rehearsal, not a performance.

                A driver does not learn on a busy highway at rush hour. They start in an empty car park with an instructor beside them. Paper trading is that empty car park. You make the same decisions, write the same notes, and face the same uncertainty, but money is not on the line.

                Treat it seriously. If you would not enter the trade with real money, do not click the practice button either.
                """,
                """
                Start by explaining the business in plain English.

                Before any chart, before any ratio, say what the company actually does. Who pays it? What problem does it solve? Why would someone choose it over the next company?

                If you cannot explain it without jargon, you do not understand it yet. Imagine describing it to a friend at a bus stop. "They sell software that helps hospitals manage appointments" is a useful sentence. "They operate a scalable platform for workflow optimisation" sounds clever and tells us very little.
                """,
                """
                Then check a few numbers like a doctor checking vitals.

                You do not need to become an accountant today. Look for four things: is revenue growing, is the company making profit, is cash moving in the right direction, and is debt becoming a problem?

                A medical checkup does not tell you everything about a person, but it tells you what to ask next. These numbers work the same way. Strong growth with no profit can be fine for a young company. Strong profit with falling cash needs questions.
                """,
                """
                Price is what you pay. Value is what you believe you are getting.

                Imagine two used cars with the same price tag. One has a clean service history and low mileage. The other has been in three accidents and needs new brakes. Same price, very different value.

                A price-to-earnings ratio is a rough way to compare a company's share price with its profit. It does not tell you if the business is good. It simply asks, "How much are people paying for each unit of profit?" A cheap number can be a bargain or a warning. Ask why.
                """,
                """
                Now let the chart and the story talk to each other.

                If the business story sounds strong but the price has been falling for months, do not shrug it off. The market may know something you do not. It may also be wrong, but you need to find out which.

                If the chart looks wonderful but the company is losing cash and debt is climbing, be careful. A nice pattern cannot repair a weak business forever. When chart and story disagree, the answer is not to pick your favourite. The answer is to investigate.
                """,
                """
                Write the trade as a small contract with yourself.

                Your contract has five lines. Entry: the price or condition that starts the trade. Invalidation: the price or event that proves you wrong. Target: where you expect the move to go and why. Size: how much you will buy based on your risk. Time stop: how long you will wait before admitting the idea went nowhere.

                Here is a made-up example. "I will buy if SPY holds above 545. I am wrong if it closes below 538. My target is 565 because that was the previous high. I risk ₦2,000, which is 400 shares at my stop distance. I will review after ten trading days." Clear, simple, testable.
                """,
                """
                Paper-trade like the notes will matter, because they will.

                Write down why you entered, how you felt, and what you saw. Take a screenshot of the chart at entry and another at exit. Your journal is not a diary of emotions for its own sake. It is a record of decisions.

                After twenty practice trades, you will start seeing patterns in yourself. Maybe you enter too early after news. Maybe you move stops when you feel nervous. Those habits are invisible until you write them down.
                """,
                """
                In the live session, we will say the review out loud.

                Each person will bring one stock and speak in three sentences. "I see..." describes the chart. "I think..." gives the idea. "I would be wrong if..." gives the invalidation.

                This sounds simple, but it exposes weak thinking fast. If you cannot finish the third sentence, the trade is not planned. If your second sentence is "it might go up", we will keep digging until there is an actual reason.
                """,
                """
                After any trade, ask three honest questions.

                Did I follow my plan? Did I risk what I said I would risk? What did I learn that I did not know before?

                Notice that profit is not one of the questions. You can make money with a bad process and lose money with a good one. We judge the process first because that is the part you control. Over time, good process gives good decisions a chance to pay.
                """,
                """
                Your first independent review is ready.

                Pick one company you have heard of. Explain the business in plain English. Check the four numbers. Read the chart on a daily time frame. Mark one support and one resistance area. Write the five-line trade contract, even if you never place the trade.

                Bring it to the next live session. We will go through it together, slowly and without embarrassment. Nobody starts this skill fully formed. The work is simply to look carefully, write clearly, and stay honest about what you do not know.
                """
            },
            Questions: new[]
            {
                new QuestionSeed(
                    "What should you do before looking at a company's chart or ratios?",
                    new[]
                    {
                        "Explain the business in plain English",
                        "Buy a small test position",
                        "Copy someone else's watchlist",
                        "Wait for a perfect indicator"
                    },
                    CorrectIndex: 0
                ),
                new QuestionSeed(
                    "What does a price-to-earnings ratio roughly compare?",
                    new[]
                    {
                        "Share price against company profit",
                        "Share price against employee count",
                        "Profit against office size",
                        "Volume against chart colour"
                    },
                    CorrectIndex: 0
                ),
                new QuestionSeed(
                    "Why does paper trading help?",
                    new[]
                    {
                        "It removes the need to learn",
                        "It guarantees real profits",
                        "It lets you practise decisions without money on the line",
                        "It makes the market move slower"
                    },
                    CorrectIndex: 2
                ),
                new QuestionSeed(
                    "Which set belongs in a trade contract?",
                    new[]
                    {
                        "Entry, invalidation, target, size, and time stop",
                        "Luck, hope, rumour, and hurry",
                        "Price, colour, friend, and feeling",
                        "Profit, celebration, screenshot, and phone call"
                    },
                    CorrectIndex: 0
                ),
                new QuestionSeed(
                    "The chart looks strong but the company is losing cash and debt is rising. What should you do?",
                    new[]
                    {
                        "Ignore the numbers and trust the chart",
                        "Investigate the disagreement",
                        "Buy immediately before the chart changes",
                        "Delete the app"
                    },
                    CorrectIndex: 1
                ),
                new QuestionSeed(
                    "What is the first question after any trade?",
                    new[]
                    {
                        "How much did I make?",
                        "Did I follow my plan?",
                        "Did I beat my friend?",
                        "Was the chart green?"
                    },
                    CorrectIndex: 1
                )
            }
        )
    };
}

public sealed record LessonSeed(
    string Title,
    string? MarketSymbol,
    IReadOnlyList<string> Slides,
    IReadOnlyList<QuestionSeed> Questions)
{
    public string OutlineContent => string.Join("\n\n\n", Slides);
}

public sealed record QuestionSeed(
    string Text,
    IReadOnlyList<string> Options,
    int CorrectIndex);
