import { SortingDirection } from '../enums/SortingDirection';
export interface ISortingOptions {
  field: string;
  direction: SortingDirection;
  priority: 0;
}