class Wallet {
  constructor(balance) {
    this.balance = balance;
    this.gold = Math.floor(this.balance / 100);
    this.silver = Math.floor((this.balance % 100) / 10);
    this.copper = Math.floor(this.balance % 10);
  }
}

export default Wallet;
