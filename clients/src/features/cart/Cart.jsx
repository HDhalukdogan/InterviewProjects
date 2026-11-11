import React, { useEffect, useState } from 'react'
import agentCart from '../../lib/agentCart';

function Cart() {
    const [products, setProducts] = useState([]);


    useEffect(() => {
        getProducts();
    }, [])

    const getProducts = async () => {
        try {
            const response = await agentCart.get('products');
            setProducts(response.data);
        } catch (error) {
            console.error('Error fetching products:', error);
            throw error;
        }
    };


    const addToCart = async (product) => {
        try {
            const response = await agentCart.put(`userCarts/addItemToUserCart?userId=01a116bd-ef79-7551-8a1f-819096434c1e`,{
                productId: product.id,
                quantity: 1,
                unitPrice: product.price
            });
            if (response.status === 200) {
                alert('Product added to cart!');
            }
        } catch (error) {
            console.error('Error adding product to cart:', error);
            throw error;
        }
    };

    return (
        <div>
            <h1>Products</h1>
            <ul>
                {products.map((item) => (
                    <li key={item.id}>
                        <h2>{item.name}</h2>
                        <p>Price: ${item.price.toFixed(2)}</p>
                        <p>Quantity: {item.quantity}</p>
                        <button onClick={() => addToCart(item)}>Add to Cart</button>
                    </li>
                ))}
            </ul>
        </div>
    );
}

export default Cart