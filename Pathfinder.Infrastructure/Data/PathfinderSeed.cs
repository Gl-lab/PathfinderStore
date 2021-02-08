using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Pathfinder.Core.Entities.Product;
using Pathfinder.Core.Entities.Account;

namespace Pathfinder.Infrastructure.Data
{
    public static class PathfinderSeed
    {
        public static async Task SeedAsync(IServiceProvider app)
        {
            var dbContext = app.GetRequiredService<PgDbContext>();
            if (!dbContext.ArticleList.Any())
            {
                await dbContext.ArticleList.AddRangeAsync(GetPreconfiguredArticles());
                await dbContext.SaveChangesAsync().ConfigureAwait(false);
            }
            if (!dbContext.DiceList.Any())
            {
                await dbContext.DiceList.AddRangeAsync(GetPreconfiguredDice());
                await dbContext.SaveChangesAsync().ConfigureAwait(false);
            }
            if (!dbContext.DamageTypeList.Any())
            {
                await dbContext.DamageTypeList.AddRangeAsync(GetPreconfiguredDamageTypeList());
                await dbContext.SaveChangesAsync().ConfigureAwait(false);
            }
            if (!dbContext.WeaponTypeList.Any())
            {
                await dbContext.WeaponTypeList.AddRangeAsync(GetPreconfiguredWeaponTypeList());
                await dbContext.SaveChangesAsync().ConfigureAwait(false);
            }
            if (!dbContext.RaceSize.Any())
            {
                await dbContext.RaceSize.AddRangeAsync(GetRaceSizes());
                await dbContext.SaveChangesAsync().ConfigureAwait(false);
            }

            if (!dbContext.Race.Any())
            {
                await dbContext.Race.AddRangeAsync(GetRaces(dbContext));
                await dbContext.SaveChangesAsync().ConfigureAwait(false);
            }
           
            if (!dbContext.Account.Any())
            {
                await dbContext.Account.AddRangeAsync(GetAccounts(dbContext));
            }
            if (!dbContext.Items.Any())
            {
                await dbContext.Items.AddRangeAsync(GetItems(dbContext));
                await dbContext.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        private static IEnumerable<RaceSize> GetRaceSizes()
        {
            return new List<RaceSize>
            {
                new RaceSize
                {
                    Id = RaceSizeType.Small,
                    Name = "Небольшой"
                },
                new RaceSize
                {
                    Id = RaceSizeType.Medium,
                    Name = "Средний"
                }
            };
        }
        private static IEnumerable<Race> GetRaces(PgDbContext dbContext)
        {
            return new List<Race>
            {
                new Race
                {
                    Name = "Гномы",
                    BaseSpeed = 20,
                    SizeId = RaceSizeType.Small
                },
                new Race
                {
                    Name = "Дварфы",
                    BaseSpeed = 20,
                    SizeId = RaceSizeType.Medium,
                    IsNightVision = true
                },
                new Race
                {
                    Name = "Люди",
                    BaseSpeed = 30,
                    SizeId = RaceSizeType.Medium,
                },
                new Race
                {
                    Name = "Полуорки",
                    BaseSpeed = 30,
                    SizeId = RaceSizeType.Medium,
                    IsNightVision = true
                },
                new Race
                {
                    Name = "Полурослики",
                    BaseSpeed = 20,
                    SizeId = RaceSizeType.Small,
                },
                new Race
                {
                    Name = "Полуэльфы",
                    BaseSpeed = 30,
                    SizeId = RaceSizeType.Medium,
                },
                new Race
                {
                    Name = "Эльфы",
                    BaseSpeed = 30,
                    SizeId = RaceSizeType.Medium,
                }
            };
        }

        private static IEnumerable<Item> GetItems(PgDbContext dbContext)
        {
            return dbContext.ArticleList.Select(e => new Item{Article = e}).ToList();
        }

        private static IEnumerable<Account> GetAccounts(PgDbContext dbContext)
        {
            return new List<Account>
            {
                new Account
                {
                    Name = "Имя",
                    Surname = "Фамилия",
                    UserId = 1,
                    Characters = GetCharacters(dbContext)
                }
            };
        }

        private static ICollection<Character> GetCharacters(PgDbContext dbContext)
        {
            var race =  dbContext.Race.First();
            return new List<Character>
            {
                new Character
                {
                    Name = "Билибой",
                    Balance = 1000,
                    Items = null,
                    Race = race,
                    Characteristics = new GroupCharacteristic
                    {
                        Strength = new Characteristic
                        {
                          Value = 20
                        },
                        Dexterity = new Characteristic
                        {
                            Value = 20
                        },
                        Charisma = new Characteristic
                        {
                            Value = 20
                        },
                        Constitution = new Characteristic
                        {
                            Value = 20
                        },
                        Intelligence  = new Characteristic
                        {
                            Value = 20
                        },
                        Wisdom = new Characteristic
                        {
                           Value = 20
                        },
                    }
                }
            };
        }

        private static IEnumerable<WeaponType> GetPreconfiguredWeaponTypeList()
        {
            return new List<WeaponType>
            {
                new WeaponType { Name = "Безоружные атаки (Простое)", ShortName = "" }
                ,new WeaponType { Name = "Легкое оружие ближнего боя (Простое)", ShortName = "" }
                ,new WeaponType { Name = "Одноручное оружие ближнего боя (Простое)", ShortName = "" }
                ,new WeaponType { Name = "Оружие дальнего боя (Простое)", ShortName = "" }
                ,new WeaponType { Name = "Двуручное оружие ближнего боя (Простое)", ShortName = "" }
                ,new WeaponType { Name = "Легкое оружие ближнего боя (Воинское)", ShortName = "" }
                ,new WeaponType { Name = "Одноручное оружие ближнего боя (Воинское)", ShortName = "" }
                ,new WeaponType { Name = "Двуручное оружие ближнего боя (Воинское)", ShortName = "" }
                ,new WeaponType { Name = "Оружие дальнего боя (Воинское)", ShortName = "" }
                ,new WeaponType { Name = "Легкое оружие ближнего боя (Экзотическое)", ShortName = "" }
                ,new WeaponType { Name = "Одноручное оружие ближнего боя (Экзотическое)", ShortName = "" }
                ,new WeaponType { Name = "Двуручное оружие ближнего боя (Экзотическое)", ShortName = "" }
                ,new WeaponType { Name = "Оружие дальнего боя (Экзотическое)", ShortName = "" }
            };
        }
        private static IEnumerable<DamageType> GetPreconfiguredDamageTypeList()
        {
            return new List<DamageType>
            {
                new DamageType { Name = "Колющее", ShortName = "К"}
                ,new DamageType { Name = "Режущие", ShortName = "Р"}
                ,new DamageType { Name = "Дробящее", ShortName = "Д"}
            };
        }
        private static IEnumerable<BaseDice> GetPreconfiguredDice()
        {
            return new List<BaseDice>()
            {
                 new BaseDice {D = 2}
                ,new BaseDice {D = 3}
                ,new BaseDice {D = 4}
                ,new BaseDice {D = 6}
                ,new BaseDice {D = 8}
                ,new BaseDice {D = 10}
                ,new BaseDice {D = 12}
                ,new BaseDice {D = 20}
                ,new BaseDice {D = 100}
            };
        }

        private static IEnumerable<Article> GetPreconfiguredArticles()
        {
            Category FirstCategory = new() { Name = "Оружие", Description = "" };
            Category SecondCategory = new() { Name = "Доспех", Description = "" };
            return new List<Article>()
            {
                new Article()
                {
                    Name = "Штык"
                    ,Category = FirstCategory
                    ,Price = 500
                },
                new Article()
                {
                    Name = "Абордажная пика"
                    ,Category = FirstCategory
                    ,Price = 800
                },
                new Article()
                {
                    Name = "Кумадэ"
                    ,Category = FirstCategory
                    ,Price = 500
                },
                new Article()
                {
                    Name = "Длинное копье"
                    ,Category = FirstCategory
                    ,Price = 500
                },
                new Article()
                {
                    Name = "Дубина с железным наконечником"
                    ,Category = FirstCategory
                    ,Price = 0
                },
                new Article()
                {
                    Name = "Стеганая ткань"
                    ,Category = SecondCategory
                    ,Price = 10000
                },
                new Article()
                {
                    Name = "Каменная дубина"
                    ,Category = FirstCategory
                    ,Price = 200
                },
                new Article()
                {
                    Name = "Тяжелая булава"
                    ,Category = FirstCategory
                    ,Price = 1200
                },
                new Article()
                {
                    Name = "Моргенштерн "
                    ,Category = FirstCategory
                    ,Price = 800
                },
                new Article()
                {
                    Name = "Короткое копье"
                    ,Category = FirstCategory
                    ,Price = 100
                }
            };
        }
    }
}
