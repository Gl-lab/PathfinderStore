import { ICategory } from './Interfaces/ICategory'
export class Category implements ICategory {
    id: number;
    name: string;
    description: string;
    constructor (id: number, name: string, description: string){
        this.id = id;
        this.name = name;
        this.description = description;
    }
}