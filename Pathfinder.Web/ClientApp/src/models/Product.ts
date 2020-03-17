import { IProduct } from './Interfaces/IProduct'
import { ICategory } from './Interfaces/ICategory'
import { Category } from './Category'
export class Product implements IProduct {
    id: number;
    name: string;
    description: string;
    price: number;
    categoryId: number;
    category: ICategory;
    constructor (){
        this.id = 0;
        this.name = '';
        this.description = '';
        this.categoryId = 0;
        this.price = 0
        this.category = new Category(0,'','');
    }

    public copyModel(target: Product) {
        this.id = target.id;
        this.categoryId = target.category.id;
        this.description = target.description;
        this.name = target.name;
        this.price = target.price;
        
    }

}