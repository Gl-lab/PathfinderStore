import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { AngularGridInstance, Column, GridOption, GraphqlService, GraphqlResult, Filters, Formatters, OnEventArgs, FieldType } from 'angular-slickgrid';

import { ProductDataService } from 'src/app/core/services/product-data.service';
import { PageService } from 'src/app/core/services/page.service';


const GRAPHQL_QUERY_DATASET_NAME = 'products';


@Component({
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.css']
})
export class ProductListComponent implements OnInit {
  angularGrid: AngularGridInstance;
  columnDefinitions: Column[];
  gridOptions: GridOption;
  dataset = [];

  constructor(private dataService: ProductDataService, private router: Router, private pageService: PageService) {
  }

  ngOnInit(): void {
    this.columnDefinitions = [
      {
        id: 'edit',
        field: 'id',
        excludeFromColumnPicker: true,
        excludeFromGridMenu: true,
        excludeFromHeaderMenu: true,
        formatter: Formatters.editIcon,
        minWidth: 30,
        maxWidth: 30,
        onCellClick: (e: Event, args: OnEventArgs) => {
          this.router.navigate(['/product/product-detail/' + args.dataContext.id]);
        }
      },
      { id: 'id', field: 'id', name: 'Id', filterable: true, sortable: true, maxWidth: 200, type: FieldType.number, filter: { model: Filters.inputNumber } },
      { id: 'name', field: 'name', name: 'Name', filterable: true, sortable: true },
      { id: 'price', field: 'price', name: 'Unit Price', filterable: true, sortable: true, maxWidth: 200, filter: { model: Filters.inputNumber } },
      { id: 'category', field: 'category.name', name: 'Category', filterable: true, sortable: true, formatter: Formatters.complexObject },
    ];

    this.gridOptions = {
      backendServiceApi: {
        service: new GraphqlService(),
        options: {
          columnDefinitions: this.columnDefinitions,
          datasetName: GRAPHQL_QUERY_DATASET_NAME
        },
        process: (query) => this.getProducts(),
      }
    };;
  }

  angularGridReady(angularGrid: AngularGridInstance) {
    this.angularGrid = angularGrid;
  }

  getProducts(): Observable<GraphqlResult> {
    var args = this.pageService.getPageArgs(this.angularGrid);

    return this.dataService.searchProducts(args)
      .pipe(map(
        page => {
          var result: GraphqlResult = {
            data: {
              [GRAPHQL_QUERY_DATASET_NAME]: [{
                nodes: page.items,
                pageInfo: {
                  hasNextPage: page.hasNextPage
                },
                totalCount: page.totalCount
              }]
            }
          };

          return result;
        }));
  }
}
