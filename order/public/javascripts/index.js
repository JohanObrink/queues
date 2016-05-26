'use strict'

function createOrder (amount) {
  const config = {
    method: 'POST',
    headers: {'Content-Type': 'application/json'},
    body: JSON.stringify({amount})
  }
  return fetch('/orders', config)
    .then(res => res.json())
    .then(json => console.log(json))
    .catch(err => console.error(err))
}

window.createOrder = createOrder
