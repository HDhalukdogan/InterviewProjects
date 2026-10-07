import { createBrowserRouter } from "react-router";
import Appointment from "../../features/appointment/Appointment";
import App from "../../App";


export const router = createBrowserRouter([
  {
    path: "/",
    element: <App />
  },
  {
    path: "/appointment",
    element: <Appointment />
  }
])