const uuid = require('uuid')
const rabbit = require('./rabbit')

function enqueuePayment (order) {
  console.log('order', order)
  order.orderNumber = uuid.v4()
  return rabbit.enqueuePayment(order)
    .then(() => order)
}

module.exports = {enqueuePayment}
