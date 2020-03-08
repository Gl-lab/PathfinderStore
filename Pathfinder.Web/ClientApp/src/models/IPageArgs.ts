import { ISortingOptions } from './ISortingOptions';
import { IFilteringOptions } from './IFilteringOptions';

export interface IPageArgs {
    pageIndex: number,
    pageSize: number,
    pagingsStrategy: PagingStrategy,
    sortingOptions: ISortingOptions[],
    filteringOptions: IFilteringOptions[]
}