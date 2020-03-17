import { Category } from './Category';
export class Product {
    constructor() {
        this.id = 0;
        this.name = '';
        this.description = '';
        this.categoryId = 0;
        this.price = 0;
        this.category = new Category(0, '', '');
    }
}
//# sourceMappingURL=Product.js.map