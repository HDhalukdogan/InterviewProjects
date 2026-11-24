import axios from 'axios';

const agentAppointment = axios.create({
    baseURL: "http://localhost:5085/api/",
    withCredentials: true // Sends IdentityApp auth cookies to DoctorAppointmentBookingAPI
});

export default agentAppointment;
