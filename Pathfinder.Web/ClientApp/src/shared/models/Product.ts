import { IProduct } from './Interfaces/IProduct';
import { ICategory } from './Interfaces/ICategory';
import { Category } from './Category';
export class Product implements IProduct {
  public id?: number;
  public name: string;
  public description: string;
  public price: number;
  public categoryId?: number;
  public category?: ICategory;
  constructor() {
    this.id = 0;
    this.name = '';
    this.description = '';
    this.categoryId = 0;
    this.price = 0;
    this.category = undefined;
  }

  public copyModel(target: Product) {
    this.id = target.id;
    this.categoryId = target.category?.id;
    this.description = target.description;
    this.name = target.name;
    this.price = target.price;

  }

}
