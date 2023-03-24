function onUpdateCourse(courseId, courseName) {
    $('#formCourseId').val(courseId);
    $('#formCourseOldName').val(courseName);
}

function onUpdateResource(id, name) {
    $('#formResourceId').val(id);
    $('#formResourceOldName').val(name);
}

function onUpdateFile(id, name) {
    console.log($('#formFileName').val());
}