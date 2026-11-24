import { createBrowserRouter } from "react-router";
import Layout from "../../components/Layout";
import App from "../../App";
import Appointment from "../../features/appointment/Appointment";
import Cart from "../../features/cart/Cart";
import WeatherForecast from "../../features/weather/WeatherForecast";
import Login from "../../features/auth/Login";
import Register from "../../features/auth/Register";
import Logout from "../../features/auth/Logout";

export const router = createBrowserRouter([
  {
    path: "/",
    element: <Layout />,
    children: [
      {
        index: true,
        element: <App />
      },
      {
        path: "appointment",
        element: <Appointment />
      },
      {
        path: "cart",
        element: <Cart />
      },
      {
        path: "weather",
        element: <WeatherForecast />
      },
      {
        path: "login",
        element: <Login />
      },
      {
        path: "register",
        element: <Register />
      },
      {
        path: "logout",
        element: <Logout />
      }
    ]
  }
]);
