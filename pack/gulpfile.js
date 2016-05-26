'use strict'
const gulp = require('gulp')
const eslint = require('gulp-eslint')
const mocha = require('gulp-mocha')

const running = {}
const watching = {}

gulp.task('lint', () => {
  running.lint = ['./**/*.js', '!./node_modules/**']
  return gulp.src(running.lint)
    .pipe(eslint())
    .pipe(eslint.format())
})

gulp.task('test', () => {
  running.test = ['./**/*.js', '!gulpfile.js', '!./node_modules/**']
  return gulp.src('./test/**/*.js')
    .pipe(mocha({reporter: 'spec'}))
})

gulp.task('watch', () => {
  Object.keys(running)
    .filter(key => !watching[key])
    .forEach(key => {
      watching[key] = true
      gulp.watch(running[key], [key])
    })
})
