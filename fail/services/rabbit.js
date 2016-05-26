const rabbit = require('rabbit.js')
const _sockets = {}
let _context

function getContext () {
  return new Promise((resolve) => {
    if (_context) {
      resolve(_context)
    } else {
      _context = rabbit.createContext(`amqp://${process.env.RABBITMQ || '192.168.99.100'}`)
      _context.on('ready', () => {
        resolve(_context)
      })
    }
  })
}

function getSocket (type, name) {
  return getContext()
    .then(ctx => new Promise((resolve) => {
      if (_sockets[type]) {
        resolve(_sockets[type])
      } else {
        _sockets[type] = ctx.socket(type, {prefetch: 1})
        _sockets[type].connect(name, () => {
          resolve(_sockets[type])
        })
      }
    }))
}

module.exports = {getSocket}
