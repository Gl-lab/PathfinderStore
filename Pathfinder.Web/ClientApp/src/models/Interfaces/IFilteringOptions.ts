import { FilteringOperator } from '../enums/FilteringOperator';
export interface IFilteringOptions{
    field: string,
    operator: FilteringOperator,
    value: any
}