import http from 'k6/http';
import { check } from 'k6';

export let options = {
    stages: [
        // { duration: '2s', target: 10 },
        // { duration: '12s', target: 500 },
        // { duration: '22s', target: 1000 },
        // { duration: '30s', target: 4000 },
        // { duration: '10s', target: 10000 },
        // { duration: '30s', target: 0 },     
          { duration: '10s', target: 2000 }, // Đột ngột 2000 users
    { duration: '30s', target: 2000 }, // Giữ 2000 users
    { duration: '10s', target: 0 },     
    ],
};

const BASE_URL = 'https://localhost:7179';
const JWT_TOKEN = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1lIjoicHBwIiwiZW1haWxhZGRyZXNzIjoibW1tQGdtYWlsLmNvbSIsImp0aSI6IjU3YzlkMDEyLTQyMTgtNDZmMi1iYmQyLTY5ZTk3ZWRhYWViYiIsInN1YiI6IjYzOTQwYTViLTA2YmMtNGE4ZS01MGYwLTA4ZGQ5NmFmYzE3NiIsInJvbGUiOiJDdXN0b21lciIsImV4cCI6MTc0NzY0NzE3NiwiaXNzIjoiRmFzaGlvblNob3AiLCJhdWQiOiJGYXNoaW9uU2hvcCJ9.Ayy6WiVTXuxO69WJmOj2OmynP9VfDR6z4-WE6Mk6y14";

export default function () {
    // GET all products
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