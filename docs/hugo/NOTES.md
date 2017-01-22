# Hugo Static Site Generator Notes

To create new content.

```shell-script
hugo new folder/contentfile
```
    
For the testing and deployment of Hugo site content, check out the script files in the root of the project called `push-latest.sh` and `test-latest.sh`.

`test-latest.sh` navigates to the hugo directory, launches Chrome for viewing the changes via http://localhost:1313 and then launches the hugo server wtih the theme and builds the drafts. When using the `push-latest.sh` script remember to change the tag in the content file (if new) drafts from true to false.

The `test-latest.sh` code.

```shell-script
#!/usr/bin/env bash
cd docs/hugo/
/usr/bin/open -a "/Applications/Google Chrome.app" --args 'http://localhost:1313' &
hugo server --theme=landing-page-hugo --buildDrafts
```

The *draft* state of a content file header.

```
+++
date = "2016-12-29T20:00:26-08:00"
title = "About"
draft = true

+++
```

Change `draft = true` to `draft = false`.

```
+++
date = "2016-12-29T20:00:26-08:00"
title = "About"
draft = true

+++
```

For deployment of the latest changes, use the `push-latest.sh` script. It actually is i the docs folder, but calls a file that is in the hugo directory, as I wrote it there originally and didn't want to change the paths used in the original script.  ;-) This file is a bit more involved then the test script. The following are the contents of the `push-latest.sh` script.

```shell-script
#!/usr/bin/env bash
# timestamp=`date mm-dd-yy-HHMM`
timestamp=$( date +%m-%d-%y-%H%M )
echo $timestamp
filename="datadiluvium-$timestamp.tgz"

echo "Generating Hugo Site with landing-page-hugo and cleaning up .DS_Store cruft.\n * * * * *"
hugo --theme=landing-page-hugo
find . -name '.DS_Store' -type f -delete
echo "Copying latest published content.\n * * * * *"
tar cvfz $filename public/
echo "Moving content to docs directory.\n * * * * *"
cp $filename ../datadiluvium.tgz

cd ..
tar -xf datadiluvium.tgz
rm -rf datadiluvium.tgz
cd ..
echo "Removing old site.\n * * * * *"
rm -rf about css font-awesome-4.1.0 fonts img index.html index.xml js services sitemap.xml
echo "Placing new site.\n * * * * *"
mv -v docs/public/* docs
cd docs
rm -rf public
cd ../
echo "Committing and pushing new site.\n * * * * *"
git add -A
git commit -m 'The latest hugo site publishing from file $filename.'
git push origin master

```

For a quick description of what's going on in the script.

1. I grab a time stamp, echo it for the start time and also passing it as part of the filename for the soon to be tarball file of the site.
2. Next the site is built with `hugo --theme=landing-page-hugo`.
3. Next the garbage .DS_Store cruft is deleted with `find . -name '.DS_Store' -type f -delete`.
4. Next the contents are compressed via `tar cvfz $filename public/`.
5. Now that is copied to the appropriate location.
6. Next uncompressed accordingly, and the copied file is deleted while the original file is left as a backup.
7. Next contents are placed in docs/public, then the public directory is removed from the documentation directory.
8. After that the appropriate steps to add, commit with a predefined commit message, and then pushed to master will the contents will be live.