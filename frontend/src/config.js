export const API_CONFIG = {
    API_BASE_URL: 'https://localhost:7297/api',
    SERVER_BASE_URL: 'https://localhost:7297',
    ENDPOINTS: {
        AUTH: {
            LOGIN: '/auth/login',
            REGISTER: '/auth/register',
            VALIDATE: '/auth/validate'
        },
        ROOMS: {
            LIST: '/room/with-images',
            DETAILS: (id) => `/room/${id}`
        },
        BOOKINGS: {
            CREATE: '/booking',
            LIST: '/booking',
            DETAILS: (id) => `/booking/${id}`,
            USER_ROOM_BOOKINGS: (roomId) => `/booking/user/room/${roomId}`
        }
    }
}; 