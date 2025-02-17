/*
    This file is part of JonDesign's SmoothGallery v1.0.1.

    JonDesign's SmoothGallery is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    JonDesign's SmoothGallery is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with JonDesign's SmoothGallery; if not, write to the Free Software
    Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA

    Main Developer: Jonathan Schemoul (JonDesign: http://www.jondesign.net/)
    Contributed code by:
    - Christian Ehret (bugfix)
	- Nitrix (bugfix)
	- Valerio from Mad4Milk for his great help with the carousel scrolling and many other things.
	- Archie Cowan for helping me find a bugfix on carousel inner width problem.
	Many thanks to:
	- The mootools team for the great mootools lib, and it's help and support throughout the project.
*/


var $removeEvents = function (object, type)
{
	if (!object.events) return object;
	if (type){
		if (!object.events[type]) return object;
		for (var fn in object.events[type]) object.removeEvent(type, fn);
		object.events[type] = null;
	} else {
		for (var evType in object.events) object.removeEvents(evType);
		object.events = null;
	}
	return object;
};
		
		
// declaring the class
var gallery = new Class({
	initialize: function(element, options) {
		this.setOptions({
			showArrows: true,
			showCarousel: false,
			showInfopane: true,
			thumbHeight: 75,
			thumbWidth: 100,
			thumbSpacing: 10,
			embedLinks: true,
			fadeDuration: 500,
			timed: false,
			delay: 6000,
			preloader: true,
			manualData: [],
			populateData: true,
			elementSelector: "div.imageElement",
			titleSelector: "h3",
			subtitleSelector: "p",
			linkSelector: "a.open",
			imageSelector: "img.full",
			thumbnailSelector: "img.thumbnail",
			slideInfoZoneOpacity: 0.7,
			carouselMinimizedOpacity: 0.4,
			carouselMinimizedHeight: 20,
			carouselMaximizedOpacity: 0.7, 
			destroyAfterPopulate: true,
			baseClass: 'jdGallery',
			withArrowsClass: 'withArrows',
			textShowCarousel: 'Pictures',
			useThumbGenerator: false,
			randomize: false
		}, options);
		this.fireEvent('onInit');
		this.currentIter = 0;
		this.lastIter = 0;
		this.maxIter = 0;
		this.galleryElement = element;
		this.galleryData = this.options.manualData;
		this.galleryInit = 1;
		this.galleryElements = Array();
		this.thumbnailElements = Array();
		this.galleryElement.addClass(this.options.baseClass);
		if (this.options.populateData)
			this.populateData();
		element.style.display="block";
		
		if (this.options.embedLinks)
		{
			this.currentLink = new Element('a').addClass('open').setProperties({
				href: '#',
				title: ''
			}).injectInside(element);
			if ((!this.options.showArrows) && (!this.options.showCarousel))
				this.galleryElement = element = this.currentLink;
			else
				this.currentLink.setStyle('display', 'none');
		}
		
		this.constructElements();
		if ((data.length>1)&&(this.options.showArrows))
		{
			var leftArrow = new Element('a').addClass('left').addEvent(
				'click',
				this.prevItem.bind(this)
			).injectInside(element);
			var rightArrow = new Element('a').addClass('right').addEvent(
				'click',
				this.nextItem.bind(this)
			).injectInside(element);
			this.galleryElement.addClass(this.options.withArrowsClass);
		}
		this.loadingElement = new Element('div').addClass('loadingElement').injectInside(element);
		if (this.options.showInfopane) this.initInfoSlideshow();
		if (this.options.showCarousel) this.initCarousel();
		this.doSlideShow(1);
	},
	populateData: function() {
		currentArrayPlace = this.galleryData.length;
		options = this.options;
		data = this.galleryData;
		this.galleryElement.getElements(options.elementSelector).each(function(el) {
			elementDict = {
				image: el.getElement(options.imageSelector).getProperty('src'),
				number: currentArrayPlace
			};
			if ((options.showInfopane) | (options.showCarousel))
				Object.extend(elementDict, {
					title: el.getElement(options.titleSelector).innerHTML,
					description: el.getElement(options.subtitleSelector).innerHTML
				});
			if (options.embedLinks)
				Object.extend(elementDict, {
					link: el.getElement(options.linkSelector).href||false,
					linkTitle: el.getElement(options.linkSelector).title||false
				});
			if ((!options.useThumbGenerator) && (options.showCarousel))
				Object.extend(elementDict, {
					thumbnail: el.getElement(options.thumbnailSelector).src
				});
			else if (options.useThumbGenerator)
				Object.extend(elementDict, {
					thumbnail: 'resizer.php?imgfile=' + elementDict.image + '&max_width=' + options.thumbWidth + '&max_height=' + options.thumbHeight
				});
			
			data[currentArrayPlace] = elementDict;
			currentArrayPlace++;
			if (this.options.destroyAfterPopulate)
				el.remove();
		});
		this.galleryData = data;
		this.fireEvent('onPopulated');
	},
	constructElements: function() {
		el = this.galleryElement;
		this.maxIter = this.galleryData.length;
		var currentImg;
		for(i=0;i<this.galleryData.length;i++)
		{
			var currentImg = new Fx.Style(
				new Element('div').addClass('slideElement').setStyles({
					'position':'absolute',
					'left':'0px',
					'right':'0px',
					'margin':'0px',
					'padding':'0px',
					'backgroundImage':"url('" + this.galleryData[i].image + "')",
					'backgroundPosition':"center center",
					'opacity':'0'
				}).injectInside(el),
				'opacity',
				{duration: this.options.fadeDuration}
			);
			this.galleryElements[parseInt(i)] = currentImg;
		}
	},
	destroySlideShow: function(element) {
		var myClassName = element.className;
		var newElement = new Element('div').addClass('myClassName');
		element.parentNode.replaceChild(newElement, element);
	},
	startSlideShow: function() {
		this.fireEvent('onStart');
		this.loadingElement.style.display = "none";
		this.lastIter = this.maxIter - 1;
		if (this.options.randomize == true)
		{
    		this.currentIter = Math.floor(Math.random() * this.maxIter);
        }
        else
        {
    		this.currentIter = 0;
        }
		this.galleryInit = 0;
		this.galleryElements[parseInt(this.currentIter)].set(1);
		if (this.options.showInfopane)
			this.showInfoSlideShow.delay(10, this);
		this.prepareTimer();
		if (this.options.embedLinks)
			this.makeLink(this.currentIter);
	},
	nextItem: function() {
		this.fireEvent('onNextCalled');
		this.nextIter = this.currentIter+1;
		if (this.nextIter >= this.maxIter)
			this.nextIter = 0;
		this.galleryInit = 0;
		this.goTo(this.nextIter);
	},
	prevItem: function() {
		this.fireEvent('onPreviousCalled');
		this.nextIter = this.currentIter-1;
		if (this.nextIter <= -1)
			this.nextIter = this.maxIter - 1;
		this.galleryInit = 0;
		this.goTo(this.nextIter);
	},
	goTo: function(num) {
		this.clearTimer();
		if (this.options.embedLinks)
			this.clearLink();
		if (this.options.showInfopane)
		{
			this.slideInfoZone.clearChain();
			this.hideInfoSlideShow().chain(this.changeItem.pass(num, this));
		} else
			this.changeItem.delay(500, this, num);
		if (this.options.embedLinks)
			this.makeLink(num);
		this.prepareTimer();
	},
	changeItem: function(num) {
		this.fireEvent('onStartChanging');
		this.galleryInit = 0;
		if (this.currentIter != num)
		{
			for(i=0;i<this.maxIter;i++)
			{
				if ((i != this.currentIter)) this.galleryElements[i].set(0);
			}
			if (num > this.currentIter) this.galleryElements[num].custom(1);
			else
			{
				this.galleryElements[num].set(1);
				this.galleryElements[this.currentIter].custom(0);
			}
			this.currentIter = num;
		}
		this.doSlideShow.bind(this)();
		this.fireEvent('onChanged');
	},
	clearTimer: function() {
		if (this.options.timed)
			$clear(this.timer);
	},
	prepareTimer: function() {
		if (this.options.timed)
			this.timer = this.nextItem.delay(this.options.delay, this);
	},
	doSlideShow: function(position) {
		if (this.galleryInit == 1)
		{
			imgPreloader = new Image();
			imgPreloader.onload=function(){
				this.startSlideShow.delay(10, this);
			}.bind(this);
			imgPreloader.src = this.galleryData[0].image;
		} else {
			if (this.options.showInfopane)
			{
				if (this.options.showInfopane)
				{
					this.showInfoSlideShow.delay((10 + this.options.fadeDuration), this);
				} else
					if (this.options.showCarousel)
						this.centerCarouselOn(position);
			}
		}
	},
	initInfoSlideshow: function() {
		/*if (this.slideInfoZone.element)
			this.slideInfoZone.element.remove();*/
		this.slideInfoZone = new Fx.Styles(new Element('div').addClass('slideInfoZone').injectInside($(this.galleryElement))).set({'opacity':0});
		var slideInfoZoneTitle = new Element('div').injectInside(this.slideInfoZone.element);
		var slideInfoZoneDescription = new Element('p').injectInside(this.slideInfoZone.element);
		this.slideInfoZone.normalHeight = this.slideInfoZone.element.offsetHeight;
		this.slideInfoZone.element.setStyle('opacity',0);
	},
	changeInfoSlideShow: function()
	{
		this.hideInfoSlideShow.delay(10, this);
		this.showInfoSlideShow.delay(500, this);
	},
	showInfoSlideShow: function() {
		this.fireEvent('onShowInfopane');
		this.slideInfoZone.clearTimer();
		element = this.slideInfoZone.element;
		element.getElement('div').setHTML(this.galleryData[this.currentIter].title);
		element.getElement('p').setHTML(this.galleryData[this.currentIter].description);
		this.slideInfoZone.custom({'opacity': [0, this.options.slideInfoZoneOpacity], 'height': [0, this.slideInfoZone.normalHeight]});
		if (this.options.showCarousel)
			this.slideInfoZone.chain(this.centerCarouselOn.pass(this.currentIter, this));
		return this.slideInfoZone;
	},
	hideInfoSlideShow: function() {
		this.fireEvent('onHideInfopane');
		this.slideInfoZone.clearTimer();
		this.slideInfoZone.custom({'opacity': 0, 'height': 0});
		return this.slideInfoZone;
	},
	makeLink: function(num) {
		this.currentLink.setProperties({
			href: this.galleryData[num].link,
			title: this.galleryData[num].linkTitle
		})
		if (!((this.options.embedLinks) && (!this.options.showArrows) && (!this.options.showCarousel)))
			this.currentLink.setStyle('display', 'block');

	},
	clearLink: function() {
		this.currentLink.setProperties({href: '', title: ''});
		if (!((this.options.embedLinks) && (!this.options.showArrows) && (!this.options.showCarousel)))
			this.currentLink.setStyle('display', 'none');
	}
});
gallery.implement(new Events);
gallery.implement(new Options);

/* All code copyright 2006 Jonathan Schemoul */