using StockTradePro.StockData.API.Models;
using Microsoft.EntityFrameworkCore;

namespace StockTradePro.StockData.API.Data
{
    public static class StockSeeder
    {
        public static async Task SeedAsync(StockDataDbContext context, ILogger logger)
        {
            try
            {
                // Check if data already exists
                if (await context.Stocks.AnyAsync())
                {
                    logger.LogInformation("Stock data already exists, skipping seeding");
                    return;
                }

                logger.LogInformation("Starting stock data seeding...");

                // Seed stocks
                var stocks = GetSeedStocks();
                await context.Stocks.AddRangeAsync(stocks);
                await context.SaveChangesAsync();

                logger.LogInformation("Seeded {Count} stocks", stocks.Count);

                // Seed initial prices for each stock
                var random = new Random();
                var stockPrices = new List<StockPrice>();

                foreach (var stock in stocks)
                {
                    // Create initial price data (last 30 days)
                    var basePrice = GetInitialPrice(stock.Symbol);

                    for (int i = 30; i >= 0; i--)
                    {
                        var date = DateTime.UtcNow.Date.AddDays(-i);
                        var price = GenerateRealisticPrice(basePrice, i, random);

                        var stockPrice = new StockPrice
                        {
                            StockId = stock.Id,
                            Date = date,
                            OpenPrice = price.Open,
                            HighPrice = price.High,
                            LowPrice = price.Low,
                            ClosePrice = price.Close,
                            CurrentPrice = price.Close,
                            Volume = GenerateVolume(stock.Symbol, random),
                            PriceChange = i == 0 ? 0 : price.Close - GetPreviousClose(stockPrices, stock.Id, date),
                            IsCurrentPrice = i == 0 // Today's price is current
                        };

                        stockPrice.PriceChangePercent = stockPrice.PriceChange != 0 && stockPrice.ClosePrice != 0
                            ? (stockPrice.PriceChange / (stockPrice.ClosePrice - stockPrice.PriceChange)) * 100
                            : 0;

                        stockPrices.Add(stockPrice);
                    }
                }

                await context.StockPrices.AddRangeAsync(stockPrices);
                await context.SaveChangesAsync();

                logger.LogInformation("Seeded {Count} stock price records", stockPrices.Count);

                // Seed market data
                var marketData = GetMarketIndices();
                await context.MarketData.AddRangeAsync(marketData);
                await context.SaveChangesAsync();

                logger.LogInformation("Seeded {Count} market indices", marketData.Count);

                logger.LogInformation("Stock data seeding completed successfully!");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred during stock data seeding");
                throw;
            }
        }

        private static List<Stock> GetSeedStocks()
        {
            return new List<Stock>
            {
                // Technology Stocks
                new Stock { Symbol = "AAPL", CompanyName = "Apple Inc.", Exchange = "NASDAQ", Sector = "Technology", Industry = "Consumer Electronics", MarketCap = 3000000000000, Description = "Apple Inc. designs, manufactures, and markets consumer electronics, computer software, and online services.", Website = "https://www.apple.com" },
                new Stock { Symbol = "MSFT", CompanyName = "Microsoft Corporation", Exchange = "NASDAQ", Sector = "Technology", Industry = "Software", MarketCap = 2800000000000, Description = "Microsoft Corporation develops, licenses, and supports software, services, devices, and solutions worldwide.", Website = "https://www.microsoft.com" },
                new Stock { Symbol = "GOOGL", CompanyName = "Alphabet Inc. Class A", Exchange = "NASDAQ", Sector = "Technology", Industry = "Internet Services", MarketCap = 1700000000000, Description = "Alphabet Inc. provides online advertising services in the United States, Europe, the Middle East, Africa, the Asia-Pacific, Canada, and Latin America.", Website = "https://www.alphabet.com" },
                new Stock { Symbol = "AMZN", CompanyName = "Amazon.com Inc.", Exchange = "NASDAQ", Sector = "Technology", Industry = "E-commerce", MarketCap = 1500000000000, Description = "Amazon.com, Inc. engages in the retail sale of consumer products and subscriptions in North America and internationally.", Website = "https://www.amazon.com" },
                new Stock { Symbol = "TSLA", CompanyName = "Tesla, Inc.", Exchange = "NASDAQ", Sector = "Technology", Industry = "Electric Vehicles", MarketCap = 800000000000, Description = "Tesla, Inc. designs, develops, manufactures, leases, and sells electric vehicles, and energy generation and storage systems.", Website = "https://www.tesla.com" },
                new Stock { Symbol = "META", CompanyName = "Meta Platforms Inc.", Exchange = "NASDAQ", Sector = "Technology", Industry = "Social Media", MarketCap = 900000000000, Description = "Meta Platforms, Inc. develops products that enable people to connect and share with friends and family through mobile devices, personal computers, virtual reality headsets, and wearables worldwide.", Website = "https://www.meta.com" },
                new Stock { Symbol = "NVDA", CompanyName = "NVIDIA Corporation", Exchange = "NASDAQ", Sector = "Technology", Industry = "Semiconductors", MarketCap = 1200000000000, Description = "NVIDIA Corporation operates as a computing company in the United States, Taiwan, China, Hong Kong, and internationally.", Website = "https://www.nvidia.com" },
                new Stock { Symbol = "NFLX", CompanyName = "Netflix Inc.", Exchange = "NASDAQ", Sector = "Technology", Industry = "Streaming Services", MarketCap = 200000000000, Description = "Netflix, Inc. provides entertainment services.", Website = "https://www.netflix.com" },
                new Stock { Symbol = "ADBE", CompanyName = "Adobe Inc.", Exchange = "NASDAQ", Sector = "Technology", Industry = "Software", MarketCap = 240000000000, Description = "Adobe Inc. operates as a diversified software company worldwide.", Website = "https://www.adobe.com" },
                new Stock { Symbol = "CRM", CompanyName = "Salesforce Inc.", Exchange = "NYSE", Sector = "Technology", Industry = "Cloud Software", MarketCap = 220000000000, Description = "Salesforce, Inc. provides customer relationship management technology that brings companies and customers together worldwide.", Website = "https://www.salesforce.com" },
                
                // Financial Services
                new Stock { Symbol = "BRK.A", CompanyName = "Berkshire Hathaway Inc. Class A", Exchange = "NYSE", Sector = "Financial Services", Industry = "Conglomerates", MarketCap = 700000000000, Description = "Berkshire Hathaway Inc., through its subsidiaries, engages in the insurance, freight rail transportation, and utility businesses worldwide.", Website = "https://www.berkshirehathaway.com" },
                new Stock { Symbol = "JPM", CompanyName = "JPMorgan Chase & Co.", Exchange = "NYSE", Sector = "Financial Services", Industry = "Banks", MarketCap = 500000000000, Description = "JPMorgan Chase & Co. operates as a financial services company worldwide.", Website = "https://www.jpmorganchase.com" },
                new Stock { Symbol = "BAC", CompanyName = "Bank of America Corporation", Exchange = "NYSE", Sector = "Financial Services", Industry = "Banks", MarketCap = 300000000000, Description = "Bank of America Corporation, through its subsidiaries, provides banking and financial products and services for individual consumers, small and middle-market businesses, institutional investors, large corporations, and governments worldwide.", Website = "https://www.bankofamerica.com" },
                new Stock { Symbol = "WFC", CompanyName = "Wells Fargo & Company", Exchange = "NYSE", Sector = "Financial Services", Industry = "Banks", MarketCap = 200000000000, Description = "Wells Fargo & Company provides banking, investment, mortgage, and consumer and commercial finance products and services.", Website = "https://www.wellsfargo.com" },
                new Stock { Symbol = "GS", CompanyName = "The Goldman Sachs Group Inc.", Exchange = "NYSE", Sector = "Financial Services", Industry = "Investment Banking", MarketCap = 150000000000, Description = "The Goldman Sachs Group, Inc., operates as an investment banking, securities, and investment management company worldwide.", Website = "https://www.goldmansachs.com" },
                
                // Healthcare
                new Stock { Symbol = "JNJ", CompanyName = "Johnson & Johnson", Exchange = "NYSE", Sector = "Healthcare", Industry = "Pharmaceuticals", MarketCap = 450000000000, Description = "Johnson & Johnson researches, develops, manufactures, and sells various products in the healthcare field worldwide.", Website = "https://www.jnj.com" },
                new Stock { Symbol = "UNH", CompanyName = "UnitedHealth Group Incorporated", Exchange = "NYSE", Sector = "Healthcare", Industry = "Health Insurance", MarketCap = 500000000000, Description = "UnitedHealth Group Incorporated operates as a diversified health care company in the United States.", Website = "https://www.unitedhealthgroup.com" },
                new Stock { Symbol = "PFE", CompanyName = "Pfizer Inc.", Exchange = "NYSE", Sector = "Healthcare", Industry = "Pharmaceuticals", MarketCap = 280000000000, Description = "Pfizer Inc. discovers, develops, manufactures, markets, distributes, and sells biopharmaceutical products worldwide.", Website = "https://www.pfizer.com" },
                new Stock { Symbol = "MRNA", CompanyName = "Moderna Inc.", Exchange = "NASDAQ", Sector = "Healthcare", Industry = "Biotechnology", MarketCap = 60000000000, Description = "Moderna, Inc., a biotechnology company, discovers, develops, and commercializes messenger RNA therapeutics and vaccines.", Website = "https://www.modernatx.com" },
                new Stock { Symbol = "TMO", CompanyName = "Thermo Fisher Scientific Inc.", Exchange = "NYSE", Sector = "Healthcare", Industry = "Medical Devices", MarketCap = 220000000000, Description = "Thermo Fisher Scientific Inc. offers life sciences solutions, analytical instruments, specialty diagnostics, and laboratory products and service worldwide.", Website = "https://www.thermofisher.com" },
                
                // Consumer Discretionary
                new Stock { Symbol = "HD", CompanyName = "The Home Depot Inc.", Exchange = "NYSE", Sector = "Consumer Discretionary", Industry = "Retail", MarketCap = 350000000000, Description = "The Home Depot, Inc. operates as a home improvement retailer.", Website = "https://www.homedepot.com" },
                new Stock { Symbol = "MCD", CompanyName = "McDonald's Corporation", Exchange = "NYSE", Sector = "Consumer Discretionary", Industry = "Restaurants", MarketCap = 200000000000, Description = "McDonald's Corporation operates and franchises McDonald's restaurants in the United States and internationally.", Website = "https://www.mcdonalds.com" },
                new Stock { Symbol = "DIS", CompanyName = "The Walt Disney Company", Exchange = "NYSE", Sector = "Consumer Discretionary", Industry = "Entertainment", MarketCap = 180000000000, Description = "The Walt Disney Company, together with its subsidiaries, operates as an entertainment company worldwide.", Website = "https://www.disney.com" },
                new Stock { Symbol = "NKE", CompanyName = "NIKE Inc.", Exchange = "NYSE", Sector = "Consumer Discretionary", Industry = "Apparel", MarketCap = 160000000000, Description = "NIKE, Inc., together with its subsidiaries, designs, develops, markets, and sells athletic footwear, apparel, equipment, and accessories worldwide.", Website = "https://www.nike.com" },
                
                // Consumer Staples
                new Stock { Symbol = "KO", CompanyName = "The Coca-Cola Company", Exchange = "NYSE", Sector = "Consumer Staples", Industry = "Beverages", MarketCap = 260000000000, Description = "The Coca-Cola Company, a beverage company, manufactures, markets, and sells various nonalcoholic beverages worldwide.", Website = "https://www.coca-colacompany.com" },
                new Stock { Symbol = "PEP", CompanyName = "PepsiCo Inc.", Exchange = "NASDAQ", Sector = "Consumer Staples", Industry = "Beverages", MarketCap = 240000000000, Description = "PepsiCo, Inc. operates as a food and beverage company worldwide.", Website = "https://www.pepsico.com" },
                new Stock { Symbol = "WMT", CompanyName = "Walmart Inc.", Exchange = "NYSE", Sector = "Consumer Staples", Industry = "Retail", MarketCap = 400000000000, Description = "Walmart Inc. engages in the operation of retail, wholesale, and other units worldwide.", Website = "https://www.walmart.com" },
                new Stock { Symbol = "PG", CompanyName = "The Procter & Gamble Company", Exchange = "NYSE", Sector = "Consumer Staples", Industry = "Household Products", MarketCap = 380000000000, Description = "The Procter & Gamble Company provides branded consumer packaged goods to consumers in North and Latin America, Europe, the Asia Pacific, Greater China, India, the Middle East, and Africa.", Website = "https://www.pg.com" },
                
                // Energy
                new Stock { Symbol = "XOM", CompanyName = "Exxon Mobil Corporation", Exchange = "NYSE", Sector = "Energy", Industry = "Oil & Gas", MarketCap = 400000000000, Description = "Exxon Mobil Corporation explores for and produces crude oil and natural gas in the United States and internationally.", Website = "https://www.exxonmobil.com" },
                new Stock { Symbol = "CVX", CompanyName = "Chevron Corporation", Exchange = "NYSE", Sector = "Energy", Industry = "Oil & Gas", MarketCap = 300000000000, Description = "Chevron Corporation, through its subsidiaries, engages in integrated energy, chemicals, and petroleum operations worldwide.", Website = "https://www.chevron.com" },
                
                // Industrials
                new Stock { Symbol = "BA", CompanyName = "The Boeing Company", Exchange = "NYSE", Sector = "Industrials", Industry = "Aerospace", MarketCap = 130000000000, Description = "The Boeing Company, together with its subsidiaries, designs, develops, manufactures, sales, services, and supports commercial jetliners, military aircraft, satellites, missile defense, human space flight and launch systems, and services worldwide.", Website = "https://www.boeing.com" },
                new Stock { Symbol = "CAT", CompanyName = "Caterpillar Inc.", Exchange = "NYSE", Sector = "Industrials", Industry = "Construction Equipment", MarketCap = 140000000000, Description = "Caterpillar Inc. manufactures construction and mining equipment, diesel and natural gas engines, industrial gas turbines, and diesel-electric locomotives worldwide.", Website = "https://www.caterpillar.com" },
                
                // Communication Services
                new Stock { Symbol = "VZ", CompanyName = "Verizon Communications Inc.", Exchange = "NYSE", Sector = "Communication Services", Industry = "Telecommunications", MarketCap = 170000000000, Description = "Verizon Communications Inc. provides communications, technology, information, and entertainment products and services to consumers, businesses, and governmental entities worldwide.", Website = "https://www.verizon.com" },
                new Stock { Symbol = "T", CompanyName = "AT&T Inc.", Exchange = "NYSE", Sector = "Communication Services", Industry = "Telecommunications", MarketCap = 130000000000, Description = "AT&T Inc. provides telecommunications, media, and technology services worldwide.", Website = "https://www.att.com" },
                
                // Additional High-Profile Stocks
                new Stock { Symbol = "AMD", CompanyName = "Advanced Micro Devices Inc.", Exchange = "NASDAQ", Sector = "Technology", Industry = "Semiconductors", MarketCap = 240000000000, Description = "Advanced Micro Devices, Inc. operates as a semiconductor company worldwide.", Website = "https://www.amd.com" },
                new Stock { Symbol = "INTC", CompanyName = "Intel Corporation", Exchange = "NASDAQ", Sector = "Technology", Industry = "Semiconductors", MarketCap = 200000000000, Description = "Intel Corporation designs, manufactures, and sells essential technologies for the cloud, smart, and connected devices for retail, industrial, and healthcare markets worldwide.", Website = "https://www.intel.com" },
                new Stock { Symbol = "ORCL", CompanyName = "Oracle Corporation", Exchange = "NYSE", Sector = "Technology", Industry = "Software", MarketCap = 320000000000, Description = "Oracle Corporation provides products and services that address enterprise information technology environments worldwide.", Website = "https://www.oracle.com" },
                new Stock { Symbol = "IBM", CompanyName = "International Business Machines Corporation", Exchange = "NYSE", Sector = "Technology", Industry = "IT Services", MarketCap = 130000000000, Description = "International Business Machines Corporation provides integrated solutions and services worldwide.", Website = "https://www.ibm.com" },
                new Stock { Symbol = "CSCO", CompanyName = "Cisco Systems Inc.", Exchange = "NASDAQ", Sector = "Technology", Industry = "Networking Equipment", MarketCap = 200000000000, Description = "Cisco Systems, Inc. designs, manufactures, and sells Internet Protocol based networking and other products related to the communications and information technology industry in the Americas, Europe, the Middle East, Africa, the Asia Pacific, Japan, and China.", Website = "https://www.cisco.com" },
                
                // More Financial Services
                new Stock { Symbol = "MA", CompanyName = "Mastercard Incorporated", Exchange = "NYSE", Sector = "Financial Services", Industry = "Payment Processing", MarketCap = 380000000000, Description = "Mastercard Incorporated, a technology company, provides transaction processing and other payment-related products and services in the United States and internationally.", Website = "https://www.mastercard.com" },
                new Stock { Symbol = "V", CompanyName = "Visa Inc.", Exchange = "NYSE", Sector = "Financial Services", Industry = "Payment Processing", MarketCap = 500000000000, Description = "Visa Inc. operates as a payments technology company worldwide.", Website = "https://www.visa.com" },
                new Stock { Symbol = "AXP", CompanyName = "American Express Company", Exchange = "NYSE", Sector = "Financial Services", Industry = "Credit Services", MarketCap = 120000000000, Description = "American Express Company, together with its subsidiaries, provides charge and credit payment card products, and travel-related services worldwide.", Website = "https://www.americanexpress.com" },
                
                // More Healthcare
                new Stock { Symbol = "ABBV", CompanyName = "AbbVie Inc.", Exchange = "NYSE", Sector = "Healthcare", Industry = "Pharmaceuticals", MarketCap = 280000000000, Description = "AbbVie Inc. discovers, develops, manufactures, and sells pharmaceuticals in the worldwide.", Website = "https://www.abbvie.com" },
                new Stock { Symbol = "LLY", CompanyName = "Eli Lilly and Company", Exchange = "NYSE", Sector = "Healthcare", Industry = "Pharmaceuticals", MarketCap = 550000000000, Description = "Eli Lilly and Company discovers, develops, and markets human pharmaceuticals worldwide.", Website = "https://www.lilly.com" },
                
                // Utilities
                new Stock { Symbol = "NEE", CompanyName = "NextEra Energy Inc.", Exchange = "NYSE", Sector = "Utilities", Industry = "Electric Utilities", MarketCap = 160000000000, Description = "NextEra Energy, Inc., through its subsidiaries, generates, transmits, distributes, and sells electric power to retail and wholesale customers in North America.", Website = "https://www.nexteraenergy.com" },
                
                // Real Estate
                new Stock { Symbol = "AMT", CompanyName = "American Tower Corporation", Exchange = "NYSE", Sector = "Real Estate", Industry = "REITs", MarketCap = 100000000000, Description = "American Tower Corporation, one of the largest global REITs, is a leading independent owner, operator and developer of multitenant communications real estate.", Website = "https://www.americantower.com" },
                
                // Materials
                new Stock { Symbol = "LIN", CompanyName = "Linde plc", Exchange = "NYSE", Sector = "Materials", Industry = "Chemicals", MarketCap = 200000000000, Description = "Linde plc operates as an industrial gas company in North and South America, Europe, the Middle East, Africa, and the Asia Pacific.", Website = "https://www.linde.com" },
                
                // More Tech Growth Stocks
                new Stock { Symbol = "PYPL", CompanyName = "PayPal Holdings Inc.", Exchange = "NASDAQ", Sector = "Technology", Industry = "Payment Processing", MarketCap = 80000000000, Description = "PayPal Holdings, Inc. operates as a technology platform and digital payments company that enables digital and mobile payments on behalf of consumers and merchants worldwide.", Website = "https://www.paypal.com" },
                new Stock { Symbol = "UBER", CompanyName = "Uber Technologies Inc.", Exchange = "NYSE", Sector = "Technology", Industry = "Ridesharing", MarketCap = 120000000000, Description = "Uber Technologies, Inc. develops and operates proprietary technology applications in the United States, Canada, Latin America, Europe, the Middle East, Africa, and the Asia Pacific.", Website = "https://www.uber.com" },
                new Stock { Symbol = "SHOP", CompanyName = "Shopify Inc.", Exchange = "NYSE", Sector = "Technology", Industry = "E-commerce", MarketCap = 80000000000, Description = "Shopify Inc., a commerce company, provides a commerce platform and services in Canada, the United States, the United Kingdom, Australia, Latin America, and internationally.", Website = "https://www.shopify.com" },
                new Stock { Symbol = "SPOT", CompanyName = "Spotify Technology S.A.", Exchange = "NYSE", Sector = "Technology", Industry = "Streaming Services", MarketCap = 30000000000, Description = "Spotify Technology S.A., together with its subsidiaries, provides audio streaming services worldwide.", Website = "https://www.spotify.com" },
                new Stock { Symbol = "ZOOM", CompanyName = "Zoom Video Communications Inc.", Exchange = "NASDAQ", Sector = "Technology", Industry = "Software", MarketCap = 20000000000, Description = "Zoom Video Communications, Inc. provides a video-first communications platform in the Americas, the Asia Pacific, Europe, the Middle East, and Africa.", Website = "https://www.zoom.us" }
            };
        }

        private static decimal GetInitialPrice(string symbol)
        {
            // Realistic price ranges for different stocks
            return symbol switch
            {
                "AAPL" => 180.00m,
                "MSFT" => 340.00m,
                "GOOGL" => 2800.00m,
                "AMZN" => 150.00m,
                "TSLA" => 240.00m,
                "META" => 320.00m,
                "NVDA" => 480.00m,
                "NFLX" => 400.00m,
                "BRK.A" => 5200.00m, 
                "JPM" => 150.00m,
                "JNJ" => 160.00m,
                "WMT" => 160.00m,
                "V" => 250.00m,
                "MA" => 400.00m,
                "UNH" => 520.00m,
                "HD" => 320.00m,
                "PG" => 150.00m,
                "XOM" => 110.00m,
                "KO" => 60.00m,
                "PEP" => 170.00m,
                "AMD" => 140.00m,
                "INTC" => 50.00m,
                "ORCL" => 110.00m,
                "CRM" => 220.00m,
                "ADBE" => 480.00m,
                _ => 100.00m // Default price
            };
        }

        private static (decimal Open, decimal High, decimal Low, decimal Close) GenerateRealisticPrice(decimal basePrice, int daysAgo, Random random)
        {
            // More recent days have less variation from current price
            var volatilityFactor = Math.Min(0.05, 0.01 + (daysAgo * 0.001));
            var dailyChange = (decimal)(random.NextDouble() - 0.5) * 2 * (decimal)volatilityFactor;

            var open = Math.Max(1, basePrice * (1 + dailyChange));
            var dailyVolatility = (decimal)(random.NextDouble() * 0.02 + 0.005); // 0.5% to 2.5% daily range

            var high = open * (1 + dailyVolatility);
            var low = open * (1 - dailyVolatility);
            var close = low + (decimal)random.NextDouble() * (high - low);

            return (Open: Math.Round(open, 2), High: Math.Round(high, 2), Low: Math.Round(low, 2), Close: Math.Round(close, 2));
        }

        private static long GenerateVolume(string symbol, Random random)
        {
            // Different volume ranges for different types of stocks
            var baseVolume = symbol switch
            {
                "AAPL" => 50000000,  // High volume stocks
                "MSFT" => 30000000,
                "TSLA" => 80000000,
                "GOOGL" => 25000000,
                "AMZN" => 35000000,
                "BRK.A" => 10000,    // Very low volume (expensive stock)
                _ => random.Next(1000000, 10000000) // Random volume for others
            };

            var variation = random.NextDouble() * 0.5 + 0.75; // 75% to 125% of base
            return (long)(baseVolume * variation);
        }

        private static decimal GetPreviousClose(List<StockPrice> existingPrices, int stockId, DateTime currentDate)
        {
            var previousPrice = existingPrices
                .Where(p => p.StockId == stockId && p.Date < currentDate)
                .OrderByDescending(p => p.Date)
                .FirstOrDefault();

            return previousPrice?.ClosePrice ?? 100.00m; // Default if no previous price
        }

        private static List<MarketData> GetMarketIndices()
        {
            return new List<MarketData>
            {
                new MarketData
                {
                    IndexName = "S&P 500",
                    IndexSymbol = "SPX",
                    CurrentValue = 4500.00m,
                    PreviousClose = 4485.00m,
                    Change = 15.00m,
                    ChangePercent = 0.33m,
                    Volume = 0,
                    IsActive = true
                },
                new MarketData
                {
                    IndexName = "NASDAQ Composite",
                    IndexSymbol = "IXIC",
                    CurrentValue = 14200.00m,
                    PreviousClose = 14150.00m,
                    Change = 50.00m,
                    ChangePercent = 0.35m,
                    Volume = 0,
                    IsActive = true
                },
                new MarketData
                {
                    IndexName = "Dow Jones Industrial Average",
                    IndexSymbol = "DJI",
                    CurrentValue = 35000.00m,
                    PreviousClose = 34950.00m,
                    Change = 50.00m,
                    ChangePercent = 0.14m,
                    Volume = 0,
                    IsActive = true
                },
                new MarketData
                {
                    IndexName = "Russell 2000",
                    IndexSymbol = "RUT",
                    CurrentValue = 2100.00m,
                    PreviousClose = 2095.00m,
                    Change = 5.00m,
                    ChangePercent = 0.24m,
                    Volume = 0,
                    IsActive = true
                }
            };
        }
    }
}