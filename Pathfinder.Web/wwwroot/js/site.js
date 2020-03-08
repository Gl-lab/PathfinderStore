// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
 new Vue({
    el: '#app',
    data: {
        items: null,
        totalPage: null,
        hasPreviousPage: false,
        hasNextPage: true,
        pageinfo: {
            "pageIndex": 1,
            "pageSize": 5,
            "pagingStrategy": 0,
            "sortingOptions": [
                {
                "field": "string",
                "direction": 0,
                "priority": 0
                }
            ],
            "filteringOptions": [
                {
                "field": "string",
                "operator": 0,
                "value": {}
                }
            ]
        }
    },
    methods: {
        nextpage: function(){
            this.pageinfo.pageIndex++;
            this.loadData();
        },
        prevpage: function(){
            this.pageinfo.pageIndex--;
            this.loadData();
        },
        loadData: function(){
            axios.post('https://localhost:5001/api/Products/SearchProducts',this.pageinfo)
                 .then(response => {
                        this.items = response.data.items;
                        this.totalPage = response.data.totalPages;
                        this.hasPreviousPage = !(response.data.pageIndex > 1);
                        this.hasNextPage = !(response.data.pageIndex < response.data.totalPages);
            });
       }
    },
    mounted() {
        this.loadData();
    }
});