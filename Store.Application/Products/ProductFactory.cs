using System;
using System.Threading.Tasks;
using Pathfinder.Core.Entities.Product;
using Pathfinder.Core.Repositories;

namespace Pathfinder.Application.Products;

public class ProductFactory
{
    private readonly IProductRepository _productRepository;

    public ProductFactory( IProductRepository productRepository )
    {
        _productRepository = productRepository;
    }

    public async Task<ProductEnriched?> Create( int id )
    {
        Product? product  = await _productRepository.GetByIdAsync( id );
        if ( product == null )
        {
            return null;
        }
        
        
        ProductEnriched productEnriched = new ProductEnriched();
        
        productEnriched.Id = product.Id;
        
        throw new NotImplementedException();
    }
}