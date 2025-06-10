import axios from "axios";
import { camelizeKeys } from 'humps';

const API_URL = "https://localhost:7297/api";

const apiClient = axios.create({
  baseURL: API_URL,
  headers: {
    "Content-Type": "application/json",
  },
});

// Перехватчик для преобразования ответов в camelCase
apiClient.interceptors.response.use(
  (response) => {
    // Преобразуем данные только если они есть и это объект
    if (response.data && typeof response.data === 'object') {
      response.data = camelizeKeys(response.data);
    }
    return response;
  },
  (error) => {
    console.error("API Error:", error);
    return Promise.reject(error);
  }
);

// Привязка токена к запросам
apiClient.interceptors.request.use(config => {
  const token = localStorage.getItem('token');
  if (token) config.headers.Authorization = `Bearer ${token}`;
  return config;
});

// export default {
//   login: (credentials) => apiClient.post("/auth/login", credentials),
//   setAuthToken: (token) => 
//     {apiClient.defaults.headers.common['Authorization'] = `Bearer ${token}`},

//   getGuests: () => apiClient.get("/guest"),
//   getGuestById: (id) => apiClient.get(`/guest/${id}`),
//   createGuest: (guestData) => apiClient.post("/guest", guestData),
//   updateGuest: (id, guestData) => apiClient.put(`/guest/${id}`, guestData),
//   deleteGuest: (id) => apiClient.delete(`/guest/${id}`),

//   getRooms: () => apiClient.get("/room"),
//   getRoomById: (id) => apiClient.get(`/room/${id}`),
//   createRoom: (roomData) => apiClient.post("/room", roomData),
//   updateRoom: (id, roomData) => apiClient.put(`/room/${id}`, roomData),
//   deleteRoom: (id) => apiClient.delete(`/room/${id}`),

//   getBookings: () => apiClient.get("/booking"),
//   getBookingById: (id) => apiClient.get(`/booking/${id}`),
//   createBooking: (bookingData) => apiClient.post("/booking", bookingData),
//   updateBooking: (id, bookingData) =>
//     apiClient.put(`/booking/${id}`, bookingData),
//   deleteBooking: (id) => apiClient.delete(`/booking/${id}`),
// };

export default apiClient;