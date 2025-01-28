(function(){ var posts=[{"u":"http://www.feedthecreature.com/posters.php","d":"FEED THE CREATURE"},{"u":"http://video.google.com/videoplay?docid=362421849901825950&amp;hl=en","d":"Really Achieving Your Childhood Dreams","t":["video"]},{"u":"http://labs.adobe.com/wiki/index.php/Thermo","d":"Thermo - Adobe Labs","t":["design","tools","adobe"]},{"u":"http://lab.mathieu-badimon.com/","d":"http://lab.mathieu-badimon.com/","t":["design","web"]},{"u":"http://blog.wired.com/sterling/2007/08/geerts-new-book.html","n":"looks good","d":"Geert\'s New Book","t":["web2.0"]}]
document.write('<style type="text/css">.delicious-posts ul{list-style-type:none}.delicious-tag,.delicious-extended{font-size:smaller}.delicious-extended{margin:0;padding:0 0 0.25em 0}.module-list-item .delicious-posts ul{margin:0;padding:0}.module-list-item .delicious-posts h2,.module-list-item .delicious-posts li:first-child{margin-top:0}.delicious-posts img{display:inline;border:0}</style>')
document.write('<div class="delicious-posts" id="delicious-posts-irhetoric"><h2 class="delicious-banner sidebar-title"><a href="http://del.icio.us"><img src="http://images.del.icio.us/static/img/delicious.small.gif" width="10" height="10" alt="del.icio.us" /></a> <a href="http://del.icio.us/irhetoric">my del.icio.us</a></h2><ul>');
for(var i=0,p;p=posts[i];i++){
 document.write('<li class="delicious-post delicious-'+(i%2?'even':'odd')+'"><a class="delicious-link"'+(p.n?(' title="'+p.n.replace(/"/g,'&quot;')+'"'):'')+' href="'+p.u+'">'+p.d+'</a> ')
 document.write('</li>')
}

document.write('</ul>');


document.write('</div>') })()
