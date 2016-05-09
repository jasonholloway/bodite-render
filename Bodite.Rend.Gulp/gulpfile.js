var gulp = require('gulp');
var exec = require('child_process').exec;

gulp.task('render', function(cb) {
   exec('C:/dev/bodite-render/Bodite.Rend.Run/bin/debug/Bodite.Rend.Run.exe C:/dev/bodite-render/Bodite.Rend.Templates', function (err, stdout, stderr) {
       
    //maybe the renderer could just output loads of json objects?
    //{path: 'adas/fef', data: 'oiuttjh'}
    //then it would be up to the gulp task to send to S3, aggregated with scripts etc
       
    console.log(stdout);
    console.log(stderr);
    cb(err);
  }); 
});

gulp.task('scripts', function(cb) {
   cb(); 
});


gulp.task('default', ['scripts', 'render']);