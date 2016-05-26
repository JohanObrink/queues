const rabbit = require('./rabbit')
const items = []

function listen () {
  return rabbit.getSocket('WORKER', 'failed-payments')
    .then(worker => {
      worker.on('data', (data) => {
        items.push(JSON.parse(data))
        worker.ack()
      })
      return worker
    })
}

module.exports = {items, listen}
