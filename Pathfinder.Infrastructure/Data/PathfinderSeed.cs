using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pathfinder.Core.Entities.Account;
using Pathfinder.Core.Entities.Product;

namespace Pathfinder.Infrastructure.Data
{
    public static class PathfinderSeed
    {
        public static async Task SeedAsync(IServiceProvider app)
        {
            var dbContext = app.GetRequiredService<PgDbContext>();
            await dbContext.Database.MigrateAsync();
            var damageTypeList = GetPreconfiguredDamageTypeList();

            Category weaponCategory = new() { CategoryType = CategoryType.Weapon, Name = "Оружие", Description = "" };
            Category armorCategory = new() { CategoryType = CategoryType.Armor, Name = "Доспех", Description = "" };
            var articles = GetPreconfiguredArticles(weaponCategory, armorCategory);
            var sizes = GetSizes();
            var items = GetItems(articles);
            var races = GetRaces();
            var weaponTypes = GetPreconfiguredWeaponTypeList();
            var weapons = GetWeapons(articles, weaponTypes, damageTypeList);
            var character = GetCharacter(races.First(), items.Take(5));
            var accounts = GetAccounts(character);
            var weaponItemProperty = GetWeaponItemProperties(items, weaponCategory);


            if (!dbContext.Size.Any())
            {
                await dbContext.Category.AddAsync(weaponCategory);
                await dbContext.Category.AddAsync(armorCategory);
                await dbContext.Size.AddRangeAsync(sizes);
                await dbContext.WeaponProficiency.AddRangeAsync(GetWeaponProficiency());
                await dbContext.DamageType.AddRangeAsync(damageTypeList);
                await dbContext.WeaponType.AddRangeAsync(weaponTypes);
                await dbContext.Product.AddRangeAsync(articles);
                await dbContext.Race.AddRangeAsync(races);
                await dbContext.Account.AddRangeAsync(accounts);
                await dbContext.Item.AddRangeAsync(items);
                await dbContext.WeaponItemProperty.AddRangeAsync(weaponItemProperty);
                await dbContext.Weapon.AddRangeAsync(weapons);

                await dbContext.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        private static IEnumerable<WeaponProficiency> GetWeaponProficiency()
        {
            return new List<WeaponProficiency>
            {
                new()
                {
                    Id = WeaponProficiencyType.Simple,
                    Name = "Простое"
                },
                new()
                {
                    Id = WeaponProficiencyType.Martial,
                    Name = "Боевое"
                },
                new()
                {
                    Id = WeaponProficiencyType.Exotic,
                    Name = "Экзотическое"
                }
            };
        }

        private static IEnumerable<Size> GetSizes()
        {
            return new List<Size>
            {
                new()
                {
                    Id = SizeType.Tiny,
                    Name = "Крошечный"
                },
                new()
                {
                    Id = SizeType.Small,
                    Name = "Небольшой"
                },
                new()
                {
                    Id = SizeType.Medium,
                    Name = "Средний"
                },
                new()
                {
                    Id = SizeType.Large,
                    Name = "Большой"
                },
                new()
                {
                    Id = SizeType.Huge,
                    Name = "Огромный"
                }
            };
        }

        private static IList<Race> GetRaces()
        {
            return new List<Race>
            {
                new()
                {
                    Name = "Гном",
                    BaseSpeed = 20,
                    SizeId = SizeType.Small
                },
                new()
                {
                    Name = "Дварф",
                    BaseSpeed = 20,
                    SizeId = SizeType.Medium,
                    IsNightVision = true
                },
                new()
                {
                    Name = "Человек",
                    BaseSpeed = 30,
                    SizeId = SizeType.Medium,
                },
                new()
                {
                    Name = "Полуорк",
                    BaseSpeed = 30,
                    SizeId = SizeType.Medium,
                    IsNightVision = true
                },
                new()
                {
                    Name = "Полурослик",
                    BaseSpeed = 20,
                    SizeId = SizeType.Small,
                },
                new()
                {
                    Name = "Полуэльф",
                    BaseSpeed = 30,
                    SizeId = SizeType.Medium,
                },
                new()
                {
                    Name = "Эльф",
                    BaseSpeed = 30,
                    SizeId = SizeType.Medium,
                }
            };
        }

        private static IList<Item> GetItems(IEnumerable<Product> articles)
        {
            return articles.Select(e => new Item { Product = e }).ToList();
        }

        private static IEnumerable<Account> GetAccounts(Character character)
        {
            return new List<Account>
            {
                new()
                {
                    Name = "Имя",
                    Surname = "Фамилия",
                    UserId = 1,
                    Characters = new List<Character> { character },
                    CurrentCharacter = null
                }
            };
        }

        private static Character GetCharacter(Race race, IEnumerable<Item> items)
        {
            var backpackItems = new List<BackpackItem>();
            backpackItems.AddRange(items.Select(item => new BackpackItem()
            {
                Item = item,
                Count = 1,
                IsWearing = false
            }));

            return new Character
            {
                Name = "Билибой",
                Backpack = new Backpack()
                {
                    Wallet = new Wallet(1000),
                    Items = backpackItems
                },
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
                    Intelligence = new Characteristic
                    {
                        Value = 20
                    },
                    Wisdom = new Characteristic
                    {
                        Value = 20
                    },
                }
            };
        }

        private static IList<WeaponType> GetPreconfiguredWeaponTypeList()
        {
            return new List<WeaponType>
            {
                new() { Name = "Безоружные атаки", ShortName = "", WeaponProficiencyId = WeaponProficiencyType.Simple },
                new()
                {
                    Name = "Легкое оружие ближнего боя", ShortName = "",
                    WeaponProficiencyId = WeaponProficiencyType.Simple
                },
                new()
                {
                    Name = "Одноручное оружие ближнего боя", ShortName = "",
                    WeaponProficiencyId = WeaponProficiencyType.Simple
                },
                new()
                {
                    Name = "Оружие дальнего боя", ShortName = "", WeaponProficiencyId = WeaponProficiencyType.Simple
                },
                new()
                {
                    Name = "Двуручное оружие ближнего боя", ShortName = "",
                    WeaponProficiencyId = WeaponProficiencyType.Simple
                },
                new()
                {
                    Name = "Легкое оружие ближнего боя", ShortName = "",
                    WeaponProficiencyId = WeaponProficiencyType.Martial
                },
                new()
                {
                    Name = "Одноручное оружие ближнего боя", ShortName = "",
                    WeaponProficiencyId = WeaponProficiencyType.Martial
                },
                new()
                {
                    Name = "Двуручное оружие ближнего боя", ShortName = "",
                    WeaponProficiencyId = WeaponProficiencyType.Martial
                },
                new()
                {
                    Name = "Оружие дальнего боя", ShortName = "", WeaponProficiencyId = WeaponProficiencyType.Martial
                },
                new()
                {
                    Name = "Легкое оружие ближнего боя", ShortName = "",
                    WeaponProficiencyId = WeaponProficiencyType.Exotic
                },
                new()
                {
                    Name = "Одноручное оружие ближнего боя", ShortName = "",
                    WeaponProficiencyId = WeaponProficiencyType.Exotic
                },
                new()
                {
                    Name = "Двуручное оружие ближнего боя", ShortName = "",
                    WeaponProficiencyId = WeaponProficiencyType.Exotic
                },
                new()
                {
                    Name = "Оружие дальнего боя", ShortName = "", WeaponProficiencyId = WeaponProficiencyType.Exotic
                }
            };
        }

        private static IList<DamageType> GetPreconfiguredDamageTypeList()
        {
            return new List<DamageType>
            {
                new() { Name = "Колющее", ShortName = "К" }, new() { Name = "Режущие", ShortName = "Р" },
                new() { Name = "Дробящее", ShortName = "Д" }
            };
        }

        private static IList<Product> GetPreconfiguredArticles(Category weaponCategory, Category armorCategory)
        {
            return new List<Product>()
            {
                new()
                {
                    Name = "Латная рукавица", Category = weaponCategory, Price = 200, Weight = 1,
                    Description =
                        "Эта металлическая рукавица позволяет вам безоружными атаками наносить смертельный урон вместо несмертельного. В остальном же удар рукой в латной рукавице по-прежнему считается безоружным. В таблице указаны цена и вес одной рукавицы. Все виды средних и тяжелых доспехов (кроме нагрудников) идут в комплекте с латными рукавицами. Латные рукавицы нельзя сбить с рук маневром разоружения."
                },
                new()
                {
                    Name = "Кинжал", Category = weaponCategory, Price = 200, Weight = 1,
                    Description =
                        "Кинжал — это клинок около фута длиной. Вы получаете +2 к проверкам Ловкости рук при попытке спрятать кинжал на себе"
                },
                new()
                {
                    Name = "Длинное копье", Category = weaponCategory, Price = 500, Weight = 9
                },
                new()
                {
                    Name = "Дубинка", Category = weaponCategory, Price = 0, Weight = 3
                },
                new()
                {
                    Name = "Стеганый доспех", Category = armorCategory, Price = 10000,
                    Description =
                        "Этот доспех, по сути, просто набивная одежда из нескольких слоев ткани, способная обеспечить лишь самую минимальную защиту."
                },
                new()
                {
                    Name = "Моргенштерн", Category = weaponCategory, Price = 800, Weight = 6,
                    Description = "Тяжелый шипованный шар, насаженный на длинную крепкую рукоять."
                },
            };
        }

        private static IEnumerable<Weapon> GetWeapons(IEnumerable<Product> items,
            IEnumerable<WeaponType> weaponTypes,
            IEnumerable<DamageType> damageTypes)
        {
            return new List<Weapon>
            {
                new()
                {
                    Product = items.First(e => e.Name == "Латная рукавица"),
                    CritRange = 20,
                    MultiplierCrit = 2,
                    WeaponType = weaponTypes.First(e => e.Name == "Безоружные атаки"),
                    SmallSizeDamage = new Dices() { Count = 1, D = Dice.D2 },
                    MediumSizeDamage = new Dices() { Count = 1, D = Dice.D3 },
                    DamageTypeList = damageTypes.Where(e => e.ShortName == "Д").ToList(),
                },
                new()
                {
                    Product = items.First(e => e.Name == "Кинжал"),
                    CritRange = 19,
                    MultiplierCrit = 2,
                    WeaponType = weaponTypes.First(e => e.Name == "Легкое оружие ближнего боя"),
                    SmallSizeDamage = new Dices() { Count = 1, D = Dice.D3 },
                    MediumSizeDamage = new Dices() { Count = 1, D = Dice.D4 },
                    DamageTypeList = damageTypes.Where(e => e.ShortName is "К" or "Р").ToList(),
                    Range = 10
                },
                new()
                {
                    Product = items.First(e => e.Name == "Длинное копье"),
                    CritRange = 20,
                    MultiplierCrit = 3,
                    WeaponType = weaponTypes.First(e => e.Name == "Двуручное оружие ближнего боя"),
                    SmallSizeDamage = new Dices() { Count = 1, D = Dice.D6 },
                    MediumSizeDamage = new Dices() { Count = 1, D = Dice.D8 },
                    DamageTypeList = damageTypes.Where(e => e.ShortName == "П").ToList()
                },
                new()
                {
                    Product = items.First(e => e.Name == "Дубинка"),
                    CritRange = 20,
                    MultiplierCrit = 2,
                    WeaponType = weaponTypes.First(e => e.Name == "Одноручное оружие ближнего боя"),
                    SmallSizeDamage = new Dices() { Count = 1, D = Dice.D4 },
                    MediumSizeDamage = new Dices() { Count = 1, D = Dice.D6 },
                    DamageTypeList = damageTypes.Where(e => e.ShortName == "Д").ToList()
                },
                new()
                {
                    Product = items.First(e => e.Name == "Моргенштерн"),
                    CritRange = 20,
                    MultiplierCrit = 2,
                    WeaponType = weaponTypes.First(e => e.Name == "Одноручное оружие ближнего боя"),
                    SmallSizeDamage = new Dices() { Count = 1, D = Dice.D6 },
                    MediumSizeDamage = new Dices() { Count = 1, D = Dice.D8 },
                    DamageTypeList = damageTypes.Where(e => e.ShortName is "Д" or "П").ToList()
                }
            };
        }

        private static IEnumerable<WeaponItemProperty> GetWeaponItemProperties(IEnumerable<Item> items,
            Category weaponCategory)
        {
            var weaponList = items
                .Where(i => i.Product.Category == weaponCategory)
                .ToList();
            return weaponList.Select(item => new WeaponItemProperty()
            {
                Item = item,
                IsMasterful = false,
                Size = SizeType.Medium,
                AdditionalDamages = null
            }).ToList();
        }
    }
}