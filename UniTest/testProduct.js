import http from 'k6/http';
import { check } from 'k6';

// export let options = {
//     vus: 5000, // Number of concurrent virtual users (adjust between 2000-3000)
//     duration: '3s', // Test duration (2-3 seconds)
// };
export let options = {
    stages: [
        { duration: '2s', target: 100 }, // Ramp-up to 2000 users over 2 seconds
        { duration: '3s', target: 2000 }, // Stay at 2000 users for 3 seconds
        { duration: '2s', target: 2500},
        { duration: '2s', target: 5000},
        { duration: '2s', target: 2500},
        { duration: '5s', target: 6000},
        { duration: '5s', target: 10000},

         // Stay at 2000 users for 3 seconds
        { duration: '2s', target: 0},    // Ramp-down to 0 users over 2 seconds
            // Ramp-down to 0 users over 2 seconds
    ],
};

const BASE_URL = 'https://localhost:7179';
const JWT_TOKEN = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1lIjoiZGFzZCIsImVtYWlsYWRkcmVzcyI6Im9rZWFhQGdtYWlsLmNvbSIsImp0aSI6Ijg4ZmY3Y2RiLTljMGQtNGU4Mi1hMGJjLWM4MTk3Y2JkNDg1MyIsInN1YiI6IjAyM2M1ZGVmLWQwZmUtNGNlNy0zMWY3LTA4ZGQ5NGVmNzU2ZiIsInJvbGUiOiJDdXN0b21lciIsImV4cCI6MTc0NzQ2MDE4MywiaXNzIjoiRmFzaGlvblNob3AiLCJhdWQiOiJGYXNoaW9uU2hvcCJ9.WCRNMjieG5EkFnXdGvkwfAPpvLJCyNFSmpErdHT4svc';

export default function () {
    // Example: GET all products
    let res = http.get(`${BASE_URL}/api/products/8415fd7e-bf94-4872-a22c-0e7322136a5f`, {
        headers: {
            'Authorization': `Bearer ${JWT_TOKEN}`,
            'Content-Type': 'application/json'
        },
        timeout: '60s'
    });

    check(res, {
        'status is 200': (r) => r.status === 200
    });
}