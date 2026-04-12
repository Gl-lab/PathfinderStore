using System;
using System.Threading.Tasks;
using Pathfinder.Store.Application.Repositories;

namespace Pathfinder.Store.Application.Products;

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