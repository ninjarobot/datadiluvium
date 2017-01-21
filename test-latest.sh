#!/usr/bin/env bash
cd docs/hugo/
/usr/bin/open -a "/Applications/Google Chrome.app" --args 'http://localhost:1313' &
hugo server --theme=landing-page-hugo --buildDrafts
