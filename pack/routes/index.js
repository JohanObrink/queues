const express = require('express')
const router = express.Router()
const listener = require('../services/queuelistener')

/* GET home page. */
router.get('/', (req, res, next) => {
  res.render('index', { title: 'Pack', items: listener.items })
})

module.exports = router
