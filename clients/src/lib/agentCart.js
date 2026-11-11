import axios from 'axios';


const agentCart = axios.create({
    baseURL: "http://localhost:5193/api/"
});



export default agentCart;