const chai = require('chai')
const expect = chai.expect
const sinon = require('sinon')
const proxyquire = require('proxyquire')

chai.use(require('sinon-chai'))
require('sinon-as-promised')

describe('services/rabbit', () => {
  let rabbit, rabbitjs, context, socket
  beforeEach(() => {
    socket = {
      connect: sinon.stub().yields(),
      write: sinon.spy()
    }
    context = {
      on: sinon.stub(),
      socket: sinon.stub().returns(socket)
    }
    context.on.withArgs('ready').yields()
    rabbitjs = {
      createContext: sinon.stub().returns(context)
    }
    rabbit = proxyquire(`${process.cwd()}/services/rabbit`, {
      'rabbit.js': rabbitjs
    })
  })
  describe('enqueueOrder', () => {
    it('creates a context with correct default host', () => {
      process.env.RABBITMQ = ''
      return rabbit.enqueuePayment({})
        .then(() => {
          expect(rabbitjs.createContext)
            .calledOnce
            .calledWith('amqp://192.168.99.100')
        })
    })
    it('creates a context with correct host', () => {
      process.env.RABBITMQ = 'snelhest'
      return rabbit.enqueuePayment({})
        .then(() => {
          expect(rabbitjs.createContext)
            .calledOnce
            .calledWith('amqp://snelhest')
        })
    })
    it('creates a socket of the correct type', () => {
      return rabbit.enqueuePayment({})
      .then(() => {
        expect(context.socket)
          .calledOnce
          .calledWith('PUSH')
      })
    })
    it('connects the socket to the correct name', () => {
      return rabbit.enqueuePayment({})
      .then(() => {
        expect(socket.connect)
          .calledOnce
          .calledWith('payments')
      })
    })
    it('publishes the order', () => {
      const order = {foo: 'bar'}
      return rabbit.enqueuePayment(order)
      .then(() => {
        expect(socket.write)
          .calledOnce
          .calledWith('{"foo":"bar"}')
      })
    })
  })
})
