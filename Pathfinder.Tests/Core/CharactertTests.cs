using System;
using System.Collections.Generic;
using Pathfinder.Core.Entities.Account;
using Pathfinder.Core.Exceptions;
using Xunit;

namespace Pathfinder.Tests.Core
{
    public class CharacterTests
    {
        private static Character GetCharacter()
        {
            return new Character {
                Name = "Билибой",
                Backpack = new Backpack()
                {
                    Wallet = new Wallet(1000), 
                    Items = new List<BackpackItem>()
                }, 
                Race = new Race
                {
                    Name = "Эльф",
                    BaseSpeed = 30,
                    SizeId = SizeType.Medium,
                },
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
            };
        }

        [Fact]
        public void CharacterWalletIncreaseBalanceTest()
        {
            var character = GetCharacter();
            var balance = character.Backpack.Wallet.IncreaseBalance(100);
            Assert.Equal(1100,character.Backpack.Wallet.Balance);
            Assert.Equal(1100,balance);
        }
        
        [Fact]
        public void CharacterWalletIncorrectIncreaseBalanceTest()
        {
            var character = GetCharacter();
            void Action() => character.Backpack.Wallet.IncreaseBalance(-100);
            var exception = Assert.Throws<CoreException>(Action);
            Assert.Equal("unacceptable value for IncreaseBalance", exception.Message);
        }
        
        [Fact]
        public void CharacterWalletDecreaseBalanceTest()
        {
            var character = GetCharacter();
            var balance = character.Backpack.Wallet.DecreaseBalance(100);
            Assert.Equal(900,character.Backpack.Wallet.Balance);
            Assert.Equal(900,balance);
        }
        
        [Fact]
        public void CharacterWalletSubZeroDecreaseBalanceTest()
        {
            var character = GetCharacter();
            void Action() => character.Backpack.Wallet.DecreaseBalance(1100);
            var exception = Assert.Throws<CoreException>(Action);
            Assert.Equal("unacceptable value for DecreaseBalance", exception.Message);
        }
        
        [Fact]
        public void CharacterWalletIncorrectDecreaseBalanceTest()
        {
            var character = GetCharacter();
            void Action() => character.Backpack.Wallet.DecreaseBalance(-100);
            var exception = Assert.Throws<CoreException>(Action);
            Assert.Equal("unacceptable value for DecreaseBalance", exception.Message);
        }
    }
}