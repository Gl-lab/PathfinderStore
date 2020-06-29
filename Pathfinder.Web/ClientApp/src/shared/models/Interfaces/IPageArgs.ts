import { ISortingOptions } from './ISortingOptions';
import { IFilteringOptions } from './IFilteringOptions';
import { PagingStrategy } from '../enums/PagingStrategy';

export interface IPageArgs {
  pageIndex: number;
  pageSize: number;
  pagingsStrategy: PagingStrategy;
  sortingOptions: ISortingOptions[];
  filteringOptions: IFilteringOptions[];
}
