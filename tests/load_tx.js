
import http from 'k6/http';
import { sleep } from 'k6';

export const options = {
  stages: [
    { duration: '30s', target: 20 },
    { duration: '1m', target: 100 },
    { duration: '30s', target: 0 },
  ],
};

export default function () {
  const url = 'http://localhost:5000/transactions';
  const payload = JSON.stringify({
    DebitAccount: 'cash',
    CreditAccount: 'revenue',
    Amount: 1
  });
  const params = { headers: { 'Content-Type': 'application/json' } };
  http.post(url, payload, params);
  sleep(1);
}
