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
    { duration: '30s', target: 10000 }, // Giữ 2000 users
    { duration: '10s', target: 0 },     
    ],
};

const BASE_URL = 'https://localhost:7179';
const JWT_TOKEN = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1lIjoiYWRtaW4iLCJlbWFpbGFkZHJlc3MiOiJhZG1pbkBnbWFpbC5jb20iLCJqdGkiOiJjN2NjYjZkNy03YWJjLTRmY2EtODA0ZC02N2Q2OWY0ODVhODgiLCJzdWIiOiIxODAxYjQ4NC0zMzVlLTQ2ZDktMGZjYi0wOGRkODYyNmY0ZGIiLCJyb2xlIjoiQWRtaW4iLCJleHAiOjE3NDg5MTc2NDIsImlzcyI6IkZhc2hpb25TaG9wIiwiYXVkIjoiRmFzaGlvblNob3AifQ.tpUn1CfaNukwzicMXaBefUibKCzQ792hPfRz06qTN2o";

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