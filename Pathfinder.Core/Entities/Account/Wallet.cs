using Pathfinder.Core.Entities.Base;
using Pathfinder.Core.Exceptions;

namespace Pathfinder.Core.Entities.Account
{
    public class Wallet: Entity
    {
        public Wallet(int balance = 0)
        {
            Balance = balance;
        }
        public int Balance { get; private set; }
        public int IncreaseBalance(int value)
        {
            if (value > 0)
                Balance += value;
            else
                throw new CoreException("unacceptable value for IncreaseBalance");
            
            return Balance;
        }
        
        public int DecreaseBalance(int value)
        {
            if (value > 0 && Balance >= value) 
                Balance -= value;
            else 
                throw new CoreException("unacceptable value for DecreaseBalance");
            
            return Balance;
        }
        
    }
}