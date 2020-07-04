import { Component, Vue } from 'vue-property-decorator';
// import { IProduct } from '../shared/models/Interfaces/IProduct';
import CategoryComboBox from '../components/comboBox/CategoryComboBox.vue';
import { Product } from '../shared/models/Product';

@Component({
  components: {
    ['cb'] : CategoryComboBox,
  },
})

export default class CreateProductForm extends Vue {
  private product: Product = new Product();
  private resultCode: number = 0;
  private errorText: string = '';
  private createProduct(): void {
    const productModel: Product = new Product();
    productModel.copyModel(this.product);
    productModel.category = undefined;
    if (this.product.name !== ''
            && this.product.description !== ''
            && this.product.price >= 0) {
      try {
        this.axios.post('/api/Products/CreateProduct', productModel)
                        .then((response) => this.resultCode = response.status)
                        .then(() => {
                          if (this.resultCode !== 200) {
                            this.errorText = 'Неудачно';
                          } else {
                            this.$router.push('product');
                          }
                        });
      } catch {
        this.errorText = 'Неудачно';
      }
    }


  }
}
