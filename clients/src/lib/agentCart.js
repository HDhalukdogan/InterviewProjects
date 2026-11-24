import axios from 'axios';

const agentCart = axios.create({
    baseURL: "http://localhost:5193/api/",
    withCredentials: true // Sends IdentityApp access_token (JWT) cookie to Cart.API
});

export default agentCart;
