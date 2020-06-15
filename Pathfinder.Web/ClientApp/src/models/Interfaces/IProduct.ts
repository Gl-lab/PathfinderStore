import { ICategory } from './ICategory';

export interface IProduct {
    id?: number;
    name?: string;
    description?: string;
    price?: number;
    categoryId?: number;
    category?: ICategory;
}
