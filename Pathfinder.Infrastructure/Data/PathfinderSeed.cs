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
            PgDbContext dbContext = app.GetRequiredService<PgDbContext>();
            if (!dbContext.ArticleList.Any())
            {
                dbContext.ArticleList.AddRange(GetPreconfiguredArticles());
                await dbContext.SaveChangesAsync().ConfigureAwait(false);
            }
            if (!dbContext.DiceList.Any())
            {
                dbContext.DiceList.AddRange(GetPreconfiguredDice());
                await dbContext.SaveChangesAsync().ConfigureAwait(false);
            }
            if (!dbContext.DamageTypeList.Any())
            {
                dbContext.DamageTypeList.AddRange(GetPreconfiguredDamageTypeList());
                await dbContext.SaveChangesAsync().ConfigureAwait(false);
            }
            if (!dbContext.WeaponTypeList.Any())
            {
                dbContext.WeaponTypeList.AddRange(GetPreconfiguredWeaponTypeList());
                await dbContext.SaveChangesAsync().ConfigureAwait(false);
            }
            if (!dbContext.RaceSize.Any())
            {
                dbContext.RaceSize.AddRange(GetRaceSizes());
                await dbContext.SaveChangesAsync().ConfigureAwait(false);
            }
            if (!dbContext.Race.Any())
            {
                dbContext.Race.AddRange(GetRaces(dbContext));
                await dbContext.SaveChangesAsync().ConfigureAwait(false);
            }
             if (!dbContext.Items.Any())
            {
                dbContext.Items.AddRange(GetItems(dbContext));
                await dbContext.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        private static IEnumerable<RaceSize> GetRaceSizes()
        {
            return new List<RaceSize>
            {
                new RaceSize
                {
                    Name = "Небольшой"
                },
                new RaceSize
                {
                    Name = "Средний"
                }
            };
        }
        private static IEnumerable<Race> GetRaces(PgDbContext dbContext)
        {
            RaceSize small = dbContext.RaceSize.Where(e => e.Name == "Небольшой").First();
            RaceSize middle = dbContext.RaceSize.Where(e => e.Name == "Средний").First();
            return new List<Race>
            {
                new Race
                {
                    Name = "Гномы",
                    BaseSpeed = 20,
                    Size = small
                },
                new Race
                {
                    Name = "Дварфы",
                    BaseSpeed = 20,
                    Size = middle,
                    HaveNigthVision = true
                },
                new Race
                {
                    Name = "Люди",
                    BaseSpeed = 30,
                    Size = middle
                },
                new Race
                {
                    Name = "Полуорки",
                    BaseSpeed = 30,
                    Size = middle,
                    HaveNigthVision = true
                },
                new Race
                {
                    Name = "Полурослики",
                    BaseSpeed = 20,
                    Size = small
                },
                new Race
                {
                    Name = "Полуэльфы",
                    BaseSpeed = 30,
                    Size = middle
                },
                new Race
                {
                    Name = "Эльфы",
                    BaseSpeed = 30,
                    Size = middle
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
                    UserId = new Guid("C41A7761-6645-4E2C-B99D-F9E767B9AC77"),
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
                            CharacteristicInfo = new CharacteristicInfo
                            {
                                Name = "Сила",
                                ShortName = "Сил"
                            },
                            Value = 20
                        },
                        Dexterity = new Characteristic
                        {
                            CharacteristicInfo = new CharacteristicInfo
                            {
                                Name = "Ловкость",
                                ShortName = "Лов"
                            },
                            Value = 20
                        },
                        Charisma = new Characteristic
                        {
                            CharacteristicInfo = new CharacteristicInfo
                            {
                                Name = "Харизма",
                                ShortName = "Хар"
                            },
                            Value = 20
                        },
                        Constitution = new Characteristic
                        {
                            CharacteristicInfo = new CharacteristicInfo
                            {
                                Name = "Выносливость",
                                ShortName = "Вын"
                            },
                            Value = 20
                        },
                        Intelligence  = new Characteristic
                        {
                            CharacteristicInfo = new CharacteristicInfo
                            {
                                Name = "Интелект",
                                ShortName = "Инт"
                            },
                            Value = 20
                        },
                        Wisdom = new Characteristic
                        {
                            CharacteristicInfo = new CharacteristicInfo
                            {
                                Name = "Мудрость",
                                ShortName = "Муд"
                            },
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
            Category FirstCategory = new Category { Name = "Оружие", Description = "" };
            Category SecondCategory = new Category { Name = "Доспех", Description = "" };
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
