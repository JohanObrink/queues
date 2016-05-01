'use strict'
const restify = require('restify')
const app = restify.createServer()
const port = process.env.PORT || 4000

app.get('/', (req, res, next) => {
  res.send('hello')
  next()
})

app.listen(port, err => {
  if (err) {
    console.log(err, err.stack)
    process.exit(1)
  }
  console.log('listening on', port)
})
