'use strict'

const express = require('express')
const router = express.Router()
const orderService = require('../services/order')

/* GET home page. */
router.get('/', (req, res, next) => {
  res.render('index', { title: 'Order' })
})

router.post('/orders', (req, res, next) => {
  return orderService.enqueuePayment(req.body)
    .then(order => res.send(201, order) || next())
    .catch(err => next(err))
})

module.exports = router
