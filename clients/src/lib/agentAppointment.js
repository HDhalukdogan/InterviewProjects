import axios from 'axios';


const agentAppointment = axios.create({
    baseURL: "http://localhost:5085/api/"
});



export default agentAppointment;