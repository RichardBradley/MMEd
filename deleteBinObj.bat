@echo off
rem # delete the bin & obj dirs, so they don't conflict with
rem # any subversion updates

rd /s /q MMEd\bin MMEd\obj GLTK\bin GLTK\obj
