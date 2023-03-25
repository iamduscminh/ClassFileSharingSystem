function onUpdateCourse(courseId, courseName, courseCode) {
    $('#formCourseId').val(courseId);
    $('#formCourseOldName').val(courseName);
    $('#formCourseCode').val(courseCode);
    console.log(courseCode);
}

function onUpdateResource(id, name) {
    $('#formResourceId').val(id);
    $('#formResourceOldName').val(name);
}

function onUpdateFile(id, name) {
   
}