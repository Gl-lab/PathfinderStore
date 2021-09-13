class Product {
  constructor() {
    this.id = 0;
    this.name = "";
    this.description = "";
    this.categoryType = 0;
    this.price = 0;
    this.category = null;
    this.copyModel = target => {
      this.id = target.id;
      this.categoryId = target.category?.id;
      this.description = target.description;
      this.name = target.name;
      this.price = Number.parseFloat(target.price);
    };
  }
}
export default Product;
