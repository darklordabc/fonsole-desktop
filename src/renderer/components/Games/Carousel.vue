<template>
    <div class="carousel-3d-container" :style="{ height: this.slideHeight + 'vh' }">
        <div class="carousel-3d-slider" :style="{
          width: this.slideWidth + 'vh',
          height: this.slideHeight + 'vh'
        }">
          <slot></slot>
        </div>
        <controls
          v-if="controlsVisible"
          :next-html="controlsNextHtml"
          :prev-html="controlsPrevHtml"
          :width="controlsWidth"
          :height="controlsHeight"
        ></controls>
    </div>
</template>

<script>
  import Controls from './Controls';
  import Slide from './Slide';

  export default {
    name: 'Carousel',
    components: {
      Controls,
      Slide,
    },
    props: {
      count: {
        type: [Number, String],
        default: 0,
      },
      perspective: {
        type: [Number, String],
        default: 40,
      },
      display: {
        type: Number,
        default: 5,
      },
      loop: {
        type: Boolean,
        default: true,
      },
      animationSpeed: {
        type: [Number, String],
        default: 500,
      },
      dir: {
        type: String,
        default: 'rtl',
      },
      width: {
        type: Number,
        default: 360,
      },
      height: {
        type: Number,
        default: 270,
      },
      border: {
        type: Number,
        default: 1,
      },
      space: Number,
      startIndex: {
        type: Number,
        default: 0,
      },
      clickable: {
        type: Boolean,
        default: true,
      },
      disable3d: {
        type: Boolean,
        default: false,
      },
      minSwipeDistance: {
        type: Number,
        default: 10,
      },
      inverseScaling: {
        type: [Number, String],
        default: 100,
      },
      controlsVisible: {
        type: Boolean,
        default: false,
      },
      controlsPrevHtml: {
        type: String,
        default: '&lsaquo;',
      },
      controlsNextHtml: {
        type: String,
        default: '&rsaquo;',
      },
      controlsWidth: {
        type: [String, Number],
        default: 50,
      },
      controlsHeight: {
        type: [String, Number],
        default: 50,
      },
    },
    data: () => ({
      viewport: 0,
      currentIndex: 0,
      total: 0,
      lock: false,
      dragOffset: 0,
      dragStartX: 0,
      mousedown: false,
      zIndex: 998,
    }),
    computed: {
      isLastSlide() {
        return this.currentIndex === this.total - 1;
      },
      isFirstSlide() {
        return this.currentIndex === 0;
      },
      isNextPossible() {
        return !(!this.loop && this.isLastSlide);
      },
      isPrevPossible() {
        return !(!this.loop && this.isFirstSlide);
      },
      slideWidth() {
        const vw = this.viewport;
        const sw = this.width + (this.border * 2);

        return vw < sw ? vw : sw;
      },
      slideHeight() {
        const sw = this.width + (this.border * 2);
        const sh = this.height + (this.border * 2);
        const ar = this.calculateAspectRatio(sw, sh);

        return this.slideWidth / ar;
      },
      visible() {
        const v = (this.display > this.total) ? this.total : this.display;
        return v;
      },
      hasHiddenSlides() {
        return this.total > this.visible;
      },
      leftIndices() {
        const n = this.loop ? this.visible + 1 :
                              Math.min(this.visible, this.currentIndex) + 1;
        const indices = [];

        for (let m = 1; m < n; m += 1) {
          indices.push((this.dir === 'ltr')
                      ? (this.currentIndex + m) % (this.total)
                      : (this.currentIndex - m) % (this.total));
        }


        return indices;
      },
      rightIndices() {
        const n = this.loop ? this.visible + 1 :
                              Math.min(this.visible, this.total - 1 - this.currentIndex) + 1;
        const indices = [];

        for (let m = 1; m < n; m += 1) {
          indices.push((this.dir === 'ltr')
                      ? (this.currentIndex - m) % (this.total)
                      : (this.currentIndex + m) % (this.total));
        }

        return indices;
      },
    },
    watch: {
      count() {
        this.computeData();
      },
    },

    mounted() {
      this.computeData();
      this.attachMutationObserver();

      if (!this.$isServer) {
        window.addEventListener('resize', this.setSize);

        if ('ontouchstart' in window) {
          this.$el.addEventListener('touchstart', this.handleMousedown);
          this.$el.addEventListener('touchend', this.handleMouseup);
          this.$el.addEventListener('touchmove', this.handleMousemove);
        } else {
          this.$el.addEventListener('mousedown', this.handleMousedown);
          this.$el.addEventListener('mouseup', this.handleMouseup);
          this.$el.addEventListener('mousemove', this.handleMousemove);
        }
      }
    },

    beforeDestroy() {
      if (!this.$isServer) {
        this.detachMutationObserver();

        if ('ontouchstart' in window) {
          this.$el.removeEventListener('touchmove', this.handleMousemove);
        } else {
          this.$el.removeEventListener('mousemove', this.handleMousemove);
        }

        window.removeEventListener('resize', this.setSize);
      }
    },

    methods: {
      /**
       * Go to next slide
       */
      goNext() {
        if (this.isNextPossible) {
          this.goSlide(this.isLastSlide ? 0 : this.currentIndex + 1);
        }
      },
      /**
       * Go to previous slide
       */
      goPrev() {
        if (this.isPrevPossible) {
          this.goSlide(this.isFirstSlide ? this.total - 1 : this.currentIndex - 1);
        }
      },
      /**
       * Go to slide
       *
       * @param  {String} index of slide where to go
       */
      goSlide(index) {
        this.currentIndex = (index < 0 || index > this.total - 1) ? 0 : index;
        this.lock = true;

        if (this.isLastSlide) {
          this.$emit('last-slide', this.currentIndex);
        }

        this.$emit('before-slide-change', this.currentIndex);

        setTimeout(() => this.animationEnd(), this.animationSpeed);
      },
      /**
       * Go to slide far slide
       */
      goFar(index) {
        let diff = (index === this.total - 1 && this.isFirstSlide) ?
          -1 : (index - this.currentIndex);

        if (this.isLastSlide && index === 0) {
          diff = 1;
        }

        const diff2 = (diff < 0) ? -diff : diff;
        let timeBuff = 0;
        let i = 0;

        while (i < diff2) {
          i += 1;
          const timeout = (diff2 === 1) ? 0 : (timeBuff);

          setTimeout(() => ((diff < 0) ? this.goPrev(diff2) : this.goNext(diff2)), timeout);

          timeBuff += (this.animationSpeed / (diff2));
        }
      },
      /**
       * Trigger actions when animation ends
       */
      animationEnd() {
        this.lock = false;
        this.$emit('after-slide-change', this.currentIndex);
      },
      /**
       * Trigger actions when mouse is released
       * @param  {Object} e The event object
       */
      handleMouseup() {
        this.mousedown = false;
        this.dragOffset = 0;
      },
      /**
       * Trigger actions when mouse is pressed
       * @param  {Object} e The event object
       */
      handleMousedown(e) {
        if (!e.touches) {
          e.preventDefault();
        }

        this.mousedown = true;
        this.dragStartX = ('ontouchstart' in window) ? e.touches[0].clientX : e.clientX;
      },
      /**
       * Trigger actions when mouse is pressed and then moved (mouse drag)
       * @param  {Object} e The event object
       */
      handleMousemove(e) {
        if (!this.mousedown) {
          return;
        }

        const eventPosX = ('ontouchstart' in window) ? e.touches[0].clientX : e.clientX;
        const deltaX = (this.dragStartX - eventPosX);

        this.dragOffset = deltaX;

        if (this.dragOffset > this.minSwipeDistance) {
          this.handleMouseup();
          this.goNext();
        } else if (this.dragOffset < -this.minSwipeDistance) {
          this.handleMouseup();
          this.goPrev();
        }
      },
      /**
       * A mutation observer is used to detect changes to the containing node
       * in order to keep the magnet container in sync with the height its reference node.
       */
      attachMutationObserver() {
        const MutationObserver = window.MutationObserver ||
          window.WebKitMutationObserver ||
          window.MozMutationObserver;

        if (MutationObserver) {
          const config = {
            attributes: true,
            childList: true,
            characterData: true,
          };

          this.mutationObserver = new MutationObserver(() => {
            this.$nextTick(() => {
              this.computeData();
            });
          });

          if (this.$el) {
            this.mutationObserver.observe(this.$el, config);
          }
        }
      },
      /**
       * Stop listening to mutation changes
       */
      detachMutationObserver() {
        if (this.mutationObserver) {
          this.mutationObserver.disconnect();
        }
      },
      /**
       * Get the number of slides
       * @return {Number} Number of slides
       */
      getSlideCount() {
        if (this.$slots.default !== undefined) {
          return this.$slots.default.filter(value => value.tag !== undefined).length;
        }

        return 0;
      },
      /**
       * Calculate slide with and keep defined aspect ratio
       * @return {Number} Aspect ratio number
       */
      calculateAspectRatio(width, height) {
        return Math.min(width / height);
      },
      /**
       * Re-compute the number of slides and current slide
       */
      computeData() {
        this.total = this.getSlideCount();
        this.currentIndex = this.startIndex > this.total - 1 ? this.total - 1 : this.startIndex;
        this.viewport = this.$el.clientWidth;
      },
      setSize() {
        this.$el.style.cssText += `height:${this.slideHeight}px;`;
        this.$el.childNodes[0].style.cssText += `width:${this.slideWidth}px; height:${this.slideHeight}px;`;
      },
    },
  };
</script>

<style lang="sass" scoped>
  .carousel-3d-container
    min-height: 1px
    width: 100%
    position: relative
    z-index: 0
    overflow: hidden
    box-sizing: border-box

  .carousel-3d-slider
    position: relative
    margin: 0 auto
    transform-style: preserve-3d
    perspective: 500vh
</style>
